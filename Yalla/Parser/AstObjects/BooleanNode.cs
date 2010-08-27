using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yalla.Parser.AstObjects
{
    public class BooleanNode : ObjectNode, IEquatable<BooleanNode>
    {
        public BooleanNode(bool value) : base(value)
        {
            Value = value;
        }

        public bool Value { get; private set; }

        #region IEquatable
        public static bool operator ==(BooleanNode left, BooleanNode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(BooleanNode left, BooleanNode right)
        {
            return !Equals(left, right);
        }

        public bool Equals(BooleanNode other)
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

            if (obj.GetType() != typeof(BooleanNode))
            {
                return false;
            }

            return Equals((BooleanNode)obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        } 
        #endregion
    }
}
