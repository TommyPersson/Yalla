using System;

namespace Yalla.Parser.AstObjects
{
    public class StringNode : ObjectNode, IEquatable<StringNode>
    {
        public StringNode(string value) : base(value)
        {
            Value = value;
        }

        public string Value { get; private set; }

        #region IEquatable
        public static bool operator ==(StringNode left, StringNode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StringNode left, StringNode right)
        {
            return !Equals(left, right);
        }

        public bool Equals(StringNode other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(other.Value, Value);
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

            if (obj.GetType() != typeof(StringNode))
            {
                return false;
            }

            return Equals((StringNode)obj);
        }

        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        } 
        #endregion
    }
}
