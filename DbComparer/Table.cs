using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbComparer
{
    public class Table : IEquatable<Table>
    {
        public ColumnCollection Columns { get; set; }
        public string Schema { get; set; }
        public bool UsesAnsiNulls { get; set; }

        public bool Equals(Table other)
        {
            if (other == null)
                return false;
            bool columnsEqual = false;
            if (Columns != null && other.Columns != null)
                columnsEqual = Columns.Equals(other.Columns);
            if (Columns == null && other.Columns == null)
                columnsEqual = true;
            return columnsEqual && Schema == other.Schema && UsesAnsiNulls == other.UsesAnsiNulls;
        }
    }
}
