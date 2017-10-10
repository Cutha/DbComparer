using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbComparer
{
    public class Column : IEquatable<Column>
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string SystemType { get; set; }
        public int MaxLength { get; set; }
        public int Precision { get; set; }
        public bool IsNullable { get; set; }
        public bool IsIdentity { get; set; }

        public bool Equals(Column other)
        {
            if (other == null)
                return false;
            return Name == other.Name && Type == other.Type && SystemType == other.SystemType &&
                   MaxLength == other.MaxLength && Precision == other.Precision && IsNullable == other.IsNullable &&
                   IsIdentity == other.IsIdentity;
        }

        public string Diff(Column other, ColumnDiffOption option)
        {
            StringBuilder result = new StringBuilder();
            if (Name != other.Name)
            {
                result.AppendLine("Name: " + Name + " " + ColumnDiffOption.DiffSymbol + " " + other.Name);
            }
            if (Type != other.Type)
            {
                result.AppendLine("Type: " + Type + " " + ColumnDiffOption.DiffSymbol + " " + other.Type);
            }
            if (SystemType != other.SystemType)
            {
                result.AppendLine("System type: " + SystemType + " " + ColumnDiffOption.DiffSymbol + " " + other.SystemType);
            }
            if (MaxLength != other.MaxLength)
            {
                result.AppendLine("Max length: " + MaxLength + " " + ColumnDiffOption.DiffSymbol + " " + other.MaxLength);
            }
            if (Precision != other.Precision)
            {
                result.AppendLine("Precision: " + Precision + " " + ColumnDiffOption.DiffSymbol + " " + other.Precision);
            }
            if (IsNullable != other.IsNullable)
            {
                result.AppendLine("IsNullable: " + IsNullable + " " + ColumnDiffOption.DiffSymbol + " " + other.IsNullable);
            }
            if (IsIdentity != other.IsIdentity)
            {
                result.AppendLine("IsIdentity: " + IsIdentity + " " + ColumnDiffOption.DiffSymbol + " " + other.IsIdentity);
            }
            return result.ToString();
        }
    }

    public class ColumnDiffOption
    {
        internal static string DiffSymbol { get; private set; }
        private static ColumnDiffOption o = null;
        public static ColumnDiffOption To
        {
            get
            {
                DiffSymbol = "-->";
                return o;
            }
        }
        public static ColumnDiffOption From
        {
            get
            {
                DiffSymbol = "<--";
                return o;
            }
        }
    }
}
