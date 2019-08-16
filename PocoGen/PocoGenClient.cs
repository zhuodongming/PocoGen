using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace PocoGen
{
    public class PocoGenClient
    {
        PocoGenOptions options = new PocoGenOptions();
        public PocoGenClient(Action<PocoGenOptions> setupAction = null)
        {
            setupAction?.Invoke(options);
        }

        public string GenerateAllTables(DbConnection conn)
        {
            SchemaReader reader = null;
            switch (conn.GetType().Name.ToLower())
            {
                case "mysqlconnection"://MySql
                    reader = new MySqlSchemaReader();
                    break;
                case "sqlconnection"://SQL Server
                    reader = new SqlServerSchemaReader();
                    break;
                case "sqlceserver"://SQL CE
                    reader = new SqlServerCeSchemaReader();
                    break;
                case "oracleconnection"://Oracle
                    reader = new OracleSchemaReader();
                    break;
                case "npgsqlconnection"://PostgreSQL
                    reader = new PostGreSqlSchemaReader();
                    break;
                default:
                    throw new Exception("未支持的数据库类型");
            }

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            Tables tables = reader.ReadSchema(conn);
            conn.Close();

            // Remove unrequired tables/views
            for (int i = tables.Count - 1; i >= 0; i--)
            {
                if (!options.IsIncludeViews && tables[i].IsView)//移除视图
                {
                    tables.RemoveAt(i);
                    continue;
                }
                if (options.ExcludePrefix != null)
                {
                    if (options.ExcludePrefix.Any(item => tables[i].ClassName.StartsWith(item)))//移除需排除的前缀类
                    {
                        tables.RemoveAt(i);
                        continue;
                    }
                }
            }

            var sb = new StringBuilder(100 * 1024);
            if (options.IsGenPetaPoco)
            {
                sb.AppendLine("using PetaPoco;");
            }
            if (options.IsGenNPoco)
            {
                sb.AppendLine("using NPoco;");
            }
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"namespace {options.Namespace}");
            sb.AppendLine("{");
            tables.ForEach(table => sb.Append(GenerateClass(table)));
            sb.AppendLine("}");
            return sb.ToString();
        }

        string GenerateClass(Table table)
        {
            var builder = new StringBuilder(200);
            if (options.IsGenNPoco || options.IsGenPetaPoco)
            {
                builder.AppendLine($"    [TableName(\"{table.Name}\")]");
                var pks = table.PK;
                if (pks.Count() > 0)
                {
                    if (pks.Count() > 1)
                    {
                        string strPK = String.Join(",", pks.Select(p => p.PropertyName));
                        builder.AppendLine($"    [PrimaryKey(\"{strPK}\", AutoIncrement = false)]");
                    }
                    else if (pks.First().IsAutoIncrement == true)
                    {
                        builder.AppendLine($"    [PrimaryKey(\"{pks.First().PropertyName}\", AutoIncrement = true)]");
                    }
                    else
                    {
                        builder.AppendLine($"    [PrimaryKey(\"{pks.First().PropertyName}\", AutoIncrement = false)]");
                    }
                }
            }
            builder.AppendLine($"    public class {options.ClassPrefix}{table.ClassName}{options.ClassSuffix}");
            builder.AppendLine("    {");
            foreach (var column in table.Columns)
            {
                builder.AppendLine($"        public {column.PropertyType}{CheckNullable(column)} {column.PropertyName} {{ get; set; }}//{column.Comment}");
            }
            builder.AppendLine("    }");
            builder.AppendLine();

            return builder.ToString();
        }

        string CheckNullable(Column col)
        {
            string result = "";
            if (col.IsNullable &&
                col.PropertyType != "byte[]" &&
                col.PropertyType != "string" &&
                col.PropertyType != "Microsoft.SqlServer.Types.SqlGeography" &&
                col.PropertyType != "Microsoft.SqlServer.Types.SqlGeometry"
                )
                result = "?";
            return result;
        }
    }
}
