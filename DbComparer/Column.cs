using System;
using System.Collections.Generic;
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
    }
}
