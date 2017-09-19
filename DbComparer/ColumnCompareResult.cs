using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbComparer
{
    public class ColumnCompareResult
    {
        public string Action { get; private set; }
        public string ColumnName { get; private set; }

        internal ColumnCompareResult(string action, string columnName)
        {
            Action = action;
            ColumnName = columnName;
        }
    }
}
