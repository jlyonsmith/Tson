﻿using System;
using System.Text;
using System.Globalization;
using System.Collections;

namespace TsonLibrary
{
    public class ToJsonTsonNodeVisitor : TsonNodeVisitor
    {
        private StringBuilder sb;
        private TsonNode rootNode;

        public ToJsonTsonNodeVisitor(TsonNode rootNode, string indentChars = "  ")
        {
            this.rootNode = rootNode;
        }

        public string ToJson()
        {
            sb = new StringBuilder();
            Visit(rootNode);
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

            sb.Append("{ ");

            while (e.MoveNext())
            {
                var kv = e.Current;

                if (addComma)
                    sb.Append(", ");
                else
                    addComma = true;

                Visit(kv.Key);
                sb.Append(": ");
                Visit(kv.Value);
            }

            sb.Append(" }");

            return node;
        }

        protected override TsonNode VisitArray(TsonArrayNodeBase node)
        {
            var e = ((IEnumerable)node).GetEnumerator();
            bool addComma = false;

            sb.AppendLine("[ ");

            while (e.MoveNext())
            {
                var v = e.Current;

                if (addComma)
                    sb.Append(", ");
                else
                    addComma = true;

                Visit((TsonNode)v);
            }

            sb.Append(" ]");

            return node;
        }

        protected override TsonNode VisitString(TsonStringNode node)
        {
            string s = node.Value.ToString()
                .Replace("\"", "\\\"")
                .Replace("\\", "\\\\")
                .Replace("/", "\\/")
                .Replace("\b", "\\b")
                .Replace("\f", "\\f")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");

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

