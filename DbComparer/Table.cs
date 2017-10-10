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

        /// <summary>
        /// Diff this table with another.
        /// </summary>
        /// <param name="other">The other table.</param>
        /// <returns></returns>
        public string Diff(Table other)
        {
            // Keep track of a list of the other table's column names
            List<string> destColNames = new List<string>(other.Columns.Select(c => c.Name));

            // Check column by column
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < Columns.Count; i++)
            {
                // If there are no more columns in the other table
                if (destColNames.Count == 0)
                {
                    foreach (var column in Columns.Skip(i))
                    {
                        result.AppendLine("Add column '" + column.Name + "':");
                        result.AppendLine("- Type: " + column.Type);
                        result.AppendLine("- System type: " + column.SystemType);
                        result.AppendLine("- Max length: " + column.MaxLength);
                        result.AppendLine("- Precision: " + column.Precision);
                        result.AppendLine("- Is nullable: " + column.IsNullable);
                        result.AppendLine("- Is identity: " + column.IsIdentity);
                    }
                    break;
                }
                Column srcCol = Columns[i];
                if (destColNames.Contains(srcCol.Name))
                {
                    Column destCol = other.Columns[srcCol.Name];
                    string columnDiff = destCol.Diff(srcCol, ColumnDiffOption.To);
                    if (columnDiff != "")
                    {
                        result.AppendLine("Column '" + srcCol.Name + "':");
                        result.AppendLine(columnDiff);
                    }
                    destColNames.Remove(srcCol.Name);
                }
                else
                {
                    result.Append("Add [" + srcCol.Name + "] " + srcCol.Type);
                    if (srcCol.Type == "nvarchar")
                        result.Append("(" + (srcCol.MaxLength / 2) + ")");
                    else if (srcCol.Type == "varchar")
                        result.Append("(" + srcCol.MaxLength + ")");
                    result.Append(" ");
                    if (srcCol.IsIdentity)
                        result.Append("identity(1, 1) ");
                    if (srcCol.IsNullable)
                        result.Append("null");
                    else
                        result.Append("not null");
                    result.AppendLine();
                }
            }

            // If there are extra columns left in the other table
            foreach (var colName in destColNames)
            {
                result.AppendLine("Remove column '" + colName + "'.");
            }
            return result.ToString();
        }
    }
}
