using System;

namespace PocoGen
{
    class Column
    {
        public string Name;//字段名
        public string PropertyName;//字段名
        public string PropertyType;//类型
        public bool IsPK;//是否主键
        public bool IsNullable;//是否可空
        public bool IsAutoIncrement;//是否自增
        public string Comment;//备注
    }
}
