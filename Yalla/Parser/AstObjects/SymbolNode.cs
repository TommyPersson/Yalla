using System;

namespace Yalla.Parser.AstObjects
{
    public class SymbolNode : AstNode, IEquatable<SymbolNode>
    {

        public SymbolNode(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

        #region IEquatable
        public static bool operator ==(SymbolNode left, SymbolNode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SymbolNode left, SymbolNode right)
        {
            return !Equals(left, right);
        }

        public bool Equals(SymbolNode other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(other.Name, Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != typeof(SymbolNode))
            {
                return false;
            }

            return Equals((SymbolNode) obj);
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }
        #endregion
    }
}