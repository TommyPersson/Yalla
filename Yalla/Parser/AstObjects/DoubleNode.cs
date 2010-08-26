
using System;

namespace Yalla.Parser.AstObjects
{
    public class DoubleNode : AstNode, IEquatable<DoubleNode>
    {
        public DoubleNode(double value)
        {
            Value = value;
        }

        public double Value { get; private set; }

        #region IEquatable
        public static bool operator ==(DoubleNode left, DoubleNode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(DoubleNode left, DoubleNode right)
        {
            return !Equals(left, right);
        }

        public bool Equals(DoubleNode other)
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

            if (obj.GetType() != typeof(DoubleNode))
            {
                return false;
            }

            return Equals((DoubleNode) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
        #endregion
    }
}