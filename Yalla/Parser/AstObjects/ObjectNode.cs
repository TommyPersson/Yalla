using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yalla.Parser.AstObjects
{
    public class ObjectNode : AstNode
    {
        public ObjectNode(object obj)
        {
            Object = obj;
        }

        public object Object { get; protected set; }
    }
}
