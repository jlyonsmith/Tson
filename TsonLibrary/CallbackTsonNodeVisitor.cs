using System;

namespace TsonLibrary
{
    public class CallbackTsonNodeVisitor : TsonNodeVisitor
    {
        Action<TsonNode> action;

        public CallbackTsonNodeVisitor(Action<TsonNode> action)
        {
            this.action = action;
        }

        public void VisitAll(TsonNode node)
        {
            Visit(node);
        }

        protected override TsonNode VisitArray(TsonArrayNodeBase node)
        {
            action(node);

            foreach (var childNode in node)
            {
                Visit((TsonNode)childNode);
            }

            return node;
        }

        protected override TsonNode VisitBoolean(TsonBooleanNode node)
        {
            action(node);
            return node;
        }

        protected override TsonNode VisitNull(TsonNullNode node)
        {
            action(node);
            return node;
        }

        protected override TsonNode VisitNumber(TsonNumberNode node)
        {
            action(node);
            return node;
        }

        protected override TsonNode VisitObject(TsonObjectNodeBase node)
        {
            action(node);

            foreach (var childNode in node)
            {
                Visit(childNode.Key);
                Visit(childNode.Value);
            }

            return node;
        }

        protected override TsonNode VisitRootObject(TsonObjectNodeBase node)
        {
            action(node);

            foreach (var childNode in node)
            {
                Visit(childNode.Key);
                Visit(childNode.Value);
            }

            return node;
        }

        protected override TsonNode VisitString(TsonStringNode node)
        {
            action(node);
            return node;
        }
    }
}

