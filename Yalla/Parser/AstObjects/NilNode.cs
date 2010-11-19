
using System;

namespace Yalla.Parser.AstObjects
{
    public class NilNode : AstNode, IEquatable<NilNode>
    {
        public static bool operator ==(NilNode left, NilNode right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(NilNode left, NilNode right)
        {
            return !Equals(left, right);
        }

        public bool Equals(NilNode other)
        {
            return !ReferenceEquals(null, other);
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

            if (obj.GetType() != typeof(NilNode))
            {
                return false;
            }

            return Equals((NilNode)obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
