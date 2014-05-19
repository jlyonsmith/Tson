using System;
using System.Text;
using System.Globalization;

namespace TsonLibrary
{
    public class JsvTsonNodeVisitor : TsonNodeVisitor
    {
        private StringBuilder sb;
        private TsonNode node;

        public JsvTsonNodeVisitor(TsonNode node)
        {
            this.node = node;
        }

        public override string ToString()
        {
            sb = new StringBuilder();
            Visit(node);
            return sb.ToString();
        }

        protected override TsonNode VisitRootObject(TsonRootObjectNode node)
        {
            return base.VisitObject(node);
        }

        protected override TsonNode VisitObject(TsonObjectNode node)
        {
            var e = node.KeyValues.GetEnumerator();
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

        protected override TsonNode VisitArray(TsonArrayNode node)
        {
            var e = node.Values.GetEnumerator();
            bool addComma = false;

            sb.Append("[");

            while (e.MoveNext())
            {
                var v = e.Current;

                if (addComma)
                    sb.Append(",");
                else
                    addComma = true;

                Visit(v);
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

