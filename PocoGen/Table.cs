using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PocoGen
{
    class Table
    {
        public List<Column> Columns { get; set; }
        public string Name { get; set; }
        public string Schema { get; set; }
        public bool IsView { get; set; }
        public string CleanName { get; set; }
        public string ClassName { get; set; }
        public bool Ignore { get; set; }

        public IEnumerable<Column> PK
        {
            get
            {
                return this.Columns.Where(x => x.IsPK);
            }
        }
    }
}
