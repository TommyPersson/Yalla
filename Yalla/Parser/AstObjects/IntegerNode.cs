using System;

namespace Yalla.Parser.AstObjects
{
    public class IntegerNode : ObjectNode, IEquatable<IntegerNode>
    {
        public IntegerNode(int value) : base(value)
        {
            Value = value;
        }

        public int Value { get; private set; }

        #region IEquatable
        public static bool operator ==(IntegerNode left, IntegerNode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(IntegerNode left, IntegerNode right)
        {
            return !Equals(left, right);
        }

        public bool Equals(IntegerNode other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other.Value == Value;
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

            if (obj.GetType() != typeof(IntegerNode))
            {
                return false;
            }

            return Equals((IntegerNode)obj);
        }

        public override int GetHashCode()
        {
            return Value;
        } 
        #endregion
    }
}