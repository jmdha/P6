using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.Models.JsonModels
{
    public class TableAttribute : ICloneable
    {
        public TableReferenceNode Table { get; set; }
        public string Attribute { get; set; }

        public TableAttribute(TableReferenceNode table, string attribute)
        {
            Table = table;
            Attribute = attribute;
        }
        public TableAttribute(string table, string attribute) : this(new TableReferenceNode(table), attribute) { }
        public TableAttribute() : this (new TableReferenceNode(), "") {}

        public override bool Equals(object? obj)
        {
            return obj is TableAttribute attribute &&
                   Attribute == attribute.Attribute &&
                   Table.Equals(attribute.Table);
        }

        public override int GetHashCode()
        {
            return Table.GetHashCode() + HashCode.Combine(Attribute);
        }

        public object Clone()
        {
            var tableClone = Table.Clone();
            if (tableClone is TableReferenceNode node)
                return new TableAttribute(node, Attribute);
            throw new ArgumentNullException("Could not clone");
        }

        public override string? ToString()
        {
            return $"({Table}).{Attribute}";
        }
    }
}
