using System;
using Newtonsoft.Json.Linq;
using System.Text;

namespace TsonService
{
    public class TsonJTokenNodeVisitor
    {
        private StringBuilder sb;
        private JObject root;

        public TsonJTokenNodeVisitor(JObject root)
        {
            this.root = root;
        }

        public string ToTson()
        {
            sb = new StringBuilder();

            Visit(root);

            return sb.ToString();
        }

        JToken Visit(JToken tok)
        {
            if (tok == null)
                return tok;

            if (tok is JObject)
                return VisitObject((JObject)tok);
            if (tok is JArray)
                return VisitArray((JArray)tok);
            if (tok is JValue)
                return VisitValue((JValue)tok);

            throw new NotImplementedException();
        }

        JToken VisitObject(JObject jObject)
        {
            var e = jObject.GetEnumerator();
            bool addComma = false;

            sb.Append("{");

            while (e.MoveNext())
            {
                var kv = e.Current;

                if (addComma)
                    sb.Append(", ");
                else
                    addComma = true;

                sb.Append(kv.Key);
                sb.Append(": ");
                Visit(kv.Value);
            }

            sb.Append("}");

            return jObject;
        }

        JToken VisitArray(JArray jArray)
        {
            var e = jArray.GetEnumerator();
            bool addComma = false;

            sb.Append("[");

            while (e.MoveNext())
            {
                var v = e.Current;

                if (addComma)
                    sb.Append(", ");
                else
                    addComma = true;

                Visit(v);
            }

            sb.Append("]");

            return jArray;
        }

        JToken VisitValue(JValue jValue)
        {
            if (jValue.Type == JTokenType.Boolean)
            {
                sb.Append(jValue.ToString().ToLower());
            }
            else if (jValue.Type == JTokenType.String)
            {
                string s = (string)jValue.Value;
                bool needsQuotes = 
                    (s.IndexOfAny(new char[] { '"', '\\', '/', '\b', '\f', '\n', '\r', '\t', '#', ':', ',', '[', ']', '{', '}' }) != -1);

                s = s
                    .Replace("\"", "\\\"")
                    .Replace("\\", "\\\\")
                    .Replace("/", "\\/")
                    .Replace("\b", "\\b")
                    .Replace("\f", "\\f")
                    .Replace("\n", "\\n")
                    .Replace("\r", "\\r")
                    .Replace("\t", "\\t");

                if (needsQuotes)
                {
                    sb.AppendFormat("\"{0}\"", s);
                }
                else
                {
                    sb.Append(s);
                }
            }
            else if (jValue.Type == JTokenType.Array || jValue.Type == JTokenType.Object)
            {
                Visit(jValue);
            }
            else
            {
                sb.Append(jValue.Value.ToString());
            }

            return jValue;
        }
    }
}

