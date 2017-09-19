using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbComparer
{
    public class ColumnCollection : IReadOnlyList<Column>
    {
        private readonly IList<Column> columns;

        private readonly Dictionary<string, int> columnNameMappings = new Dictionary<string, int>();

        public ColumnCollection(IList<Column> columns)
        {
            this.columns = columns;
            for (var i = 0; i < columns.Count; i++)
            {
                columnNameMappings.Add(columns[i].Name, i);
            }
        }

        /// <summary>
        /// Gets the Column object at the specified index.
        /// </summary>
        /// <param name="index">The index of the Column object.</param>
        /// <returns></returns>
        public Column this[int index]
        {
            get { return columns[index]; }
        }

        /// <summary>
        /// Gets the Column object with the given name. This is an O(1) operation.
        /// </summary>
        /// <param name="name">The column name.</param>
        /// <returns></returns>
        public Column this[string name]
        {
            get { return this[columnNameMappings[name]]; }
        }

        public int Count
        {
            get { return columns.Count; }
        }

        public IEnumerator<Column> GetEnumerator()
        {
            return columns.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
