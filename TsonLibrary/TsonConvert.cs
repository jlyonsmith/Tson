using System;
using System.Text;
using System.IO;
using System.Globalization;

namespace TsonLibrary
{
    public static class TsonConvert
    {
        public static string ToJsvString(string tson)
        {
            var node = new TsonParser().Parse(tson);
            var visitor = new JsvTsonNodeVisitor(node);

            return visitor.ToString();
        }
    }
}

