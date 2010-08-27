
using System;

namespace Yalla.Parser.AstObjects
{
    public class DecimalNode : ObjectNode, IEquatable<DecimalNode>
    {
        public DecimalNode(decimal value) : base(value)
        {
            Value = value;
        }

        public decimal Value { get; private set; }

        #region IEquatable
        public static bool operator ==(DecimalNode left, DecimalNode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DecimalNode left, DecimalNode right)
        {
            return !Equals(left, right);
        }

        public bool Equals(DecimalNode other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.Value.Equals(Value);
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

            if (obj.GetType() != typeof(DecimalNode))
            {
                return false;
            }

            return Equals((DecimalNode) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        #endregion
    }
}