
using System;

namespace Yalla.Parser.AstObjects
{
    public class ObjectNode : AstNode, IEquatable<ObjectNode>
    {
        public ObjectNode(object obj)
        {
            Object = obj;
        }

        public object Object { get; protected set; }

        #region IEquatable
        public static bool operator ==(ObjectNode left, ObjectNode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ObjectNode left, ObjectNode right)
        {
            return !Equals(left, right);
        }

        public bool Equals(ObjectNode other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(other.Object, Object);
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

            if (obj.GetType() != typeof(ObjectNode))
            {
                return false;
            }

            return Equals((ObjectNode)obj);
        }

        public override int GetHashCode()
        {
            return Object != null ? Object.GetHashCode() : 0;
        }

        #endregion
    }
}
