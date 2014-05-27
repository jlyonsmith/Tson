using System;
using TsonLibrary;
using System.IO;

namespace Tson2Jsv
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: mono PrettyTson.exe <input-file>");
                return;
            }

            try
            {
                string s = Tson.Format(File.ReadAllText(args[0]));

                Console.WriteLine(s);
            }
            catch (TsonParseException e)
            {
                Console.WriteLine("{0}({1},{2}): error: {3}", args[0], e.ErrorLocation.Line, e.ErrorLocation.Column, e.Message);
            }
        }
    }
}
