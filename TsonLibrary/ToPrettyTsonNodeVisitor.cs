using System;
using System.Text;
using System.Globalization;
using System.Collections;

namespace TsonLibrary
{
    public class ToPrettyTsonNodeVisitor : TsonNodeVisitor
    {
        private StringBuilder sb;
        private TsonNode rootNode;
        private string indentChars;
        private string indent;

        public ToPrettyTsonNodeVisitor(TsonNode rootNode, string indentChars = "  ")
        {
            this.rootNode = rootNode;
            this.indentChars = indentChars;
            this.indent = String.Empty;
        }

        public string ToTson()
        {
            sb = new StringBuilder();
            Visit(rootNode);
            return sb.ToString();
        }

        private void Indent()
        {
            indent += indentChars;
        }

        private void Outdent()
        {
            indent = indent.Substring(0, Math.Max(indent.Length / indentChars.Length - 1, 0) * indentChars.Length);
        }

        protected override TsonNode VisitRootObject(TsonObjectNodeBase node)
        {
            var e = node.GetEnumerator();
            bool addComma = false;

            while (e.MoveNext())
            {
                var kv = e.Current;

                if (addComma)
                    sb.AppendLine(",");
                else
                    addComma = true;

                sb.Append(indent);
                Visit(kv.Key);

                if (kv.Value.IsObject || kv.Value.IsArray)
                {
                    sb.AppendLine(":");
                }
                else
                {
                    sb.Append(": ");
                }

                Visit(kv.Value);
            }

            sb.AppendLine();

            return node;
        }

        protected override TsonNode VisitObject(TsonObjectNodeBase node)
        {
            var e = node.GetEnumerator();
            bool addComma = false;

            sb.Append(indent);
            sb.AppendLine("{");
            Indent();

            while (e.MoveNext())
            {
                var kv = e.Current;

                if (addComma)
                    sb.AppendLine(",");
                else
                    addComma = true;

                sb.Append(indent);
                Visit(kv.Key);

                if (kv.Value.IsObject || kv.Value.IsArray)
                {
                    sb.AppendLine(":");
                }
                else
                {
                    sb.Append(": ");
                }

                Visit(kv.Value);
            }

            sb.AppendLine();
            Outdent();
            sb.Append(indent);
            sb.Append("}");

            return node;
        }

        protected override TsonNode VisitArray(TsonArrayNodeBase node)
        {
            var e = ((IEnumerable)node).GetEnumerator();
            bool addComma = false;

            sb.Append(indent);
            sb.AppendLine("[");
            Indent();

            while (e.MoveNext())
            {
                var v = (TsonNode)e.Current;

                if (addComma)
                    sb.AppendLine(",");
                else
                    addComma = true;

                if (!(v.IsObject || v.IsArray))
                {
                    sb.Append(indent);
                }

                Visit(v);
            }

            sb.AppendLine();
            Outdent();
            sb.Append(indent);
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

