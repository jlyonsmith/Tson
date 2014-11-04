using System;
using System.Text;
using System.Globalization;
using System.Collections;

namespace TsonLibrary
{
    public class CompactNodeVisitor : TsonNodeVisitor
    {
        private StringBuilder sb;
        private TsonNode node;

        public CompactNodeVisitor(TsonNode node)
        {
            this.node = node;
        }

        public string ToTson()
        {
            sb = new StringBuilder();
            Visit(node);
            return sb.ToString();
        }

        protected override TsonNode VisitRootObject(TsonObjectNodeBase node)
        {
            return VisitObject(node);
        }

        protected override TsonNode VisitObject(TsonObjectNodeBase node)
        {
            var e = node.GetEnumerator();
            bool addComma = false;

            sb.Append("{");

            while (e.MoveNext())
            {
                var kv = e.Current;

                if (addComma)
                    sb.Append(",");
                else
                    addComma = true;

                Visit(kv.Key);
                sb.Append(":");
                Visit(kv.Value);
            }

            sb.Append("}");

            return node;
        }

        protected override TsonNode VisitArray(TsonArrayNodeBase node)
        {
            var e = ((IEnumerable)node).GetEnumerator();
            bool addComma = false;

            sb.Append("[");

            while (e.MoveNext())
            {
                var v = e.Current;

                if (addComma)
                    sb.Append(",");
                else
                    addComma = true;

                Visit((TsonNode)v);
            }

            sb.Append("]");

            return node;
        }

        protected override TsonNode VisitString(TsonStringNode node)
        {
            if (node.IsQuoted)
                sb.AppendFormat("\"{0}\"", node.Value.ToString());
            else
                sb.Append(node.Value.ToString());

            return node;
        }

        protected override TsonNode VisitNumber(TsonNumberNode node)
        {
            sb.Append(node.Value.ToString("G6", CultureInfo.InvariantCulture));
            return node;
        }

        protected override TsonNode VisitBoolean(TsonBooleanNode node)
        {
            sb.Append(node.Value.ToString().ToLower());
            return node;
        }

        protected override TsonNode VisitNull(TsonNullNode node)
        {
            sb.Append("null");
            return node;
        }
    }
}

