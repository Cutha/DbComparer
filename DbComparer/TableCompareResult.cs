using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbComparer
{
    public class TableCompareResult
    {
        public bool IsEqual { get; private set; }
        public ColumnCompareResult Columns { get; set; }

        internal TableCompareResult(bool isEqual, ColumnCompareResult columns)
        {
            IsEqual = isEqual;
            Columns = columns;
        }
    }
}
