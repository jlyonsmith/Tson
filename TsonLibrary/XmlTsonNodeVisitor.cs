using System;
using System.Text;
using System.Globalization;
using System.Collections;

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

        protected override TsonNode VisitRootObject(TsonObjectNodeBase node)
        {
            var e = node.GetEnumerator();

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

        protected override TsonNode VisitObject(TsonObjectNodeBase node)
        {
            var e = node.GetEnumerator();

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

        protected override TsonNode VisitArray(TsonArrayNodeBase node)
        {
            var e = ((IEnumerable)node).GetEnumerator();

            sb.Append("<array>");

            while (e.MoveNext())
            {
                var v = e.Current;

                sb.AppendFormat("<item>");
                Visit((TsonNode)v);
                sb.AppendFormat("</item>");
            }

            sb.Append("</array>");

            return node;
        }

        protected override TsonNode VisitString(TsonStringNode node)
        {
            string s = node.Value.ToString()
                .Replace("\"", "&quot;")
                .Replace("'", "&apos;")
                .Replace("&", "&amp;")
                .Replace("<", "&lt;")
                .Replace(">", "&gt;")
                .Replace("\b", "&#x8;")
                .Replace("\f", "&#xC;")
                .Replace("\n", "&#xA;")
                .Replace("\r", "&#xD;")
                .Replace("\t", "&#x9");

            sb.AppendFormat("\"{0}\"", s);

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

