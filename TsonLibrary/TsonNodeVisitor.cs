using System;

namespace TsonLibrary
{
    public abstract class TsonNodeVisitor
    {
        bool visitedRoot = false;

        protected virtual TsonNode Visit(TsonNode node)
        {
            if (node == null)
                return node;

            switch (node.NodeType)
            {
            case TsonNodeType.String:
                return this.VisitString((TsonStringNode)node);
            case TsonNodeType.Number:
                return this.VisitNumber((TsonNumberNode)node);
            case TsonNodeType.Boolean:
                return this.VisitBoolean((TsonBooleanNode)node);
            case TsonNodeType.Null:
                return this.VisitNull((TsonNullNode)node);
            case TsonNodeType.Array:
                return this.VisitArray((TsonArrayNodeBase)node);
            case TsonNodeType.Object:
                {
                    if (!visitedRoot)
                    {
                        node = this.VisitRootObject((TsonObjectNodeBase)node);
                        visitedRoot = true;
                        return node;
                    }
                    else
                        return this.VisitObject((TsonObjectNodeBase)node);
                }
            default:
                throw new NotSupportedException();
            }
        }

        protected virtual TsonNode VisitArray(TsonArrayNodeBase node)
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

        protected virtual TsonNode VisitObject(TsonObjectNodeBase node)
        {
            return node;
        }

        protected virtual TsonNode VisitRootObject(TsonObjectNodeBase node)
        {
            return node;
        }

        protected virtual TsonNode VisitString(TsonStringNode node)
        {
            return node;
        }
    }
}

