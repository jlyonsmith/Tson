using System;

namespace TsonLibrary
{
    public abstract class TsonNodeVisitor
    {
        protected TsonNodeVisitor()
        {
        }

        protected virtual TsonNode Visit(TsonNode node)
        {
            if (node == null)
                return node;

            switch (node.NodeType)
            {
            case TsonNodeType.Array:
                return this.VisitArray((TsonArrayNode)node);
            case TsonNodeType.Boolean:
                return this.VisitBoolean((TsonBooleanNode)node);
            case TsonNodeType.Null:
                return this.VisitNull((TsonNullNode)node);
            case TsonNodeType.Number:
                return this.VisitNumber((TsonNumberNode)node);
            case TsonNodeType.Object:
                return this.VisitObject((TsonObjectNode)node);
            case TsonNodeType.RootObject:
                return this.VisitRootObject((TsonRootObjectNode)node);
            case TsonNodeType.String:
                return this.VisitString((TsonStringNode)node);
            default:
                throw new NotImplementedException();
            }
        }

        protected virtual TsonNode VisitArray(TsonArrayNode node)
        {
            return node;
        }

        protected virtual TsonNode VisitBoolean(TsonBooleanNode node)
        {
            return node;
        }

        protected virtual TsonNode VisitNull(TsonNullNode node)
        {
            return node;
        }

        protected virtual TsonNode VisitNumber(TsonNumberNode node)
        {
            return node;
        }

        protected virtual TsonNode VisitObject(TsonObjectNode node)
        {
            return node;
        }

        protected virtual TsonNode VisitRootObject(TsonRootObjectNode node)
        {
            return node;
        }

        protected virtual TsonNode VisitString(TsonStringNode node)
        {
            return node;
        }
    }
}

