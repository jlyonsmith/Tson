using System;

namespace TsonLibrary
{
    public enum TsonFormatStyle
    {
        Pretty,
        Compact
    }

    public static class Tson
    {
        public static TsonObjectNode ToObjectNode(string tson)
        {
            return new TsonParser().Parse(tson);
        }

        public static T ToObjectNode<T>(string tson) where T : TsonTypedObjectNode, new()
        {
            var objectNode = new TsonParser().Parse(tson);

            return ToObjectNode<T>(objectNode);
        }

        public static T ToObjectNode<T>(TsonObjectNode objectNode) where T : TsonTypedObjectNode, new()
        {
            var visitor = new ToTypedTsonNodeVisitor<T>(objectNode);

            return visitor.ToTypedObjectNode();
        }

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

        public static string Format(string tson, TsonFormatStyle formatStyle)
        {
            var objectNode = new TsonParser().Parse(tson);

            return Format(objectNode, formatStyle);
        }

        public static string Format(TsonObjectNodeBase objectNode, TsonFormatStyle formatStyle)
        {
            switch (formatStyle)
            {
                case TsonFormatStyle.Pretty:
                    return new ToPrettyTsonNodeVisitor(objectNode).ToTson();
                default:
                    return new CompactNodeVisitor(objectNode).ToTson();
            }
        }

        public static string ToJson(string tson)
        {
            var objectNode = new TsonParser().Parse(tson);

            return ToJson(objectNode);
        }

        public static string ToJson(TsonObjectNodeBase objectNode)
        {
            var visitor = new ToJsonTsonNodeVisitor(objectNode);

            return visitor.ToJson();
        }

        public static string ToJsv(string tson)
        {
            var objectNode = new TsonParser().Parse(tson);

            return ToJsv(objectNode);
        }

        public static string ToJsv(TsonObjectNodeBase objectNode)
        {
            var visitor = new ToJsvTsonNodeVisitor(objectNode);

            return visitor.ToJsv();
        }

        public static string ToXml(string tson)
        {
            var objectNode = new TsonParser().Parse(tson);

            return ToXml(objectNode);
        }

        public static string ToXml(TsonObjectNodeBase objectNode)
        {
            var visitor = new XmlTsonNodeVisitor(objectNode);

            return visitor.ToXml();
        }
    }
}

