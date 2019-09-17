using System;
using System.Collections.Generic;
using System.Text;

namespace Aix.DBToES
{
    public class ESOptions
    {
        public string[] Urls { get; set; }

        /// <summary>
        /// create,index,update,delete
        /// </summary>
        public string opType { get; set; } = "index";

        /// <summary>
        /// 每次处理多少条 默认1000条
        /// </summary>
        public int PerBatchSize { get; set; } = 1000;

        /// <summary>
        /// 目标索引
        /// </summary>
        public string Index { get; set; }

        /// <summary>
        /// 目标类型 默认 _doc
        /// </summary>
        public string Type { get; set; } = "_doc";
    }

    public class DBOptions
    {
        public string dbType { get; set; } = "mysql";

        public string connectionString { get; set; }

        public string Sql { get; set; }

        public string PrimaryKey { get; set; }

        public string OrderBy { get; set; }
    }
}
