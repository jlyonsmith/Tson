using System;

namespace TsonLibrary
{
    public static class TsonSerializer
    {
        public static T Deserialize<T>(string tson) where T:class
        {
            var node = new TsonParser().Parse(tson);
            var t = new DeserializeTsonNodeVisitor<T>(node).ToInstance();

            return t;
        }

        public static string Serialize<T>(T data)
        {
            var node = new SerializePropertyVisitor(data).ToTsonRootObjectNode();
            var tson = new CompactNodeVisitor(node).ToString();

            return tson;
        }
    }
}

