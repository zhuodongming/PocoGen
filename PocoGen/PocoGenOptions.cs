using System;
using System.Collections.Generic;
using System.Text;

namespace PocoGen
{
    public class PocoGenOptions
    {
        /// <summary>
        /// 命名空间
        /// </summary>
        public string Namespace { get; set; } = "DefaultNameSpace";

        /// <summary>
        /// 类前缀
        /// </summary>
        public string ClassPrefix { get; set; }

        /// <summary>
        /// 类后缀
        /// </summary>
        public string ClassSuffix { get; set; }

        /// <summary>
        /// 是否包含数据库视图
        /// </summary>
        public bool IsIncludeViews { get; set; }

        /// <summary>
        /// 是否生成NPoco
        /// </summary>
        public bool IsGenNPoco { get; set; }

        /// <summary>
        /// 是否生成NPoco
        /// </summary>
        public bool IsGenPetaPoco { get; set; }

        /// <summary>
        /// 排除的前缀表
        /// </summary>
        public string[] ExcludePrefix { get; set; }
    }
}
