using System;

namespace TsonLibrary
{
    public static class Tson
    {
        public static bool Validate(string tson)
        {
            try
            {
                new TsonParser().Parse(tson);

                return true;
            }
            catch (TsonParseException)
            {
                return false;
            }
        }

        public static string Format(string tson)
        {
            var node = new TsonParser().Parse(tson);
            var visitor = new PrettyTsonNodeVisitor(node);

            return visitor.ToPrettyTson();
        }

        public static string ToJson(string tson)
        {
            var node = new TsonParser().Parse(tson);
            var visitor = new JsonTsonNodeVisitor(node);

            return visitor.ToJson();
        }

        public static string ToJsv(string tson)
        {
            var node = new TsonParser().Parse(tson);
            var visitor = new JsvTsonNodeVisitor(node);

            return visitor.ToJsv();
        }

        public static string ToXml(string tson)
        {
            var node = new TsonParser().Parse(tson);
            var visitor = new XmlTsonNodeVisitor(node);

            return visitor.ToXml();
        }
    }
}

