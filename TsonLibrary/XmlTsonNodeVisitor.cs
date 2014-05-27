using System;
using System.Text;
using System.Globalization;

namespace TsonLibrary
{
    public class XmlTsonNodeVisitor : TsonNodeVisitor
    {
        private StringBuilder sb;
        private TsonNode rootNode;

        public XmlTsonNodeVisitor(TsonNode rootNode)
        {
            this.rootNode = rootNode;
        }

        public string ToXml()
        {
            sb = new StringBuilder();
            Visit(rootNode);
            return sb.ToString();
        }

        protected override TsonNode VisitRootObject(TsonRootObjectNode node)
        {
            var e = node.KeyValues.GetEnumerator();

            sb.Append("<root>");

            while (e.MoveNext())
            {
                var kv = e.Current;

                sb.AppendFormat("<{0}>", kv.Key.Value);
                Visit(kv.Value);
                sb.AppendFormat("</{0}>", kv.Key.Value);
            }

            sb.Append("</root>");

            return node;
        }

        protected override TsonNode VisitObject(TsonObjectNode node)
        {
            var e = node.KeyValues.GetEnumerator();

            sb.Append("<object>");

            while (e.MoveNext())
            {
                var kv = e.Current;

                sb.AppendFormat("<{0}>", kv.Key.Value);
                Visit(kv.Value);
                sb.AppendFormat("</{0}>", kv.Key.Value);
            }

            sb.Append("</object>");

            return node;
        }

        protected override TsonNode VisitArray(TsonArrayNode node)
        {
            var e = node.Values.GetEnumerator();

            sb.Append("<array>");

            while (e.MoveNext())
            {
                var v = e.Current;

                sb.AppendFormat("<item>");
                Visit(v);
                sb.AppendFormat("</item>");
            }

            sb.Append("</array>");

            return node;
        }

        protected override TsonNode VisitString(TsonStringNode node)
        {
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

