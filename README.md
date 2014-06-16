## TSON: Typeable Simple Object Notation

### Rational

TSON is a less rigid form of JSON to make it as easy for humans to type as possible.  It makes a great data storage format for any type of data that humans will edit.  It supports:

- Comments using the simplest comment convention around; the # symbol.
- All strings can be left unquoted, provided they don't contain the characters `",:[]{}`
- Real newlines and tabs in strings are perfectly OK.
- If your string needs any of the reserved 7 TSON characters, or is empty, just put double quotes around it.
- In addition, double quoted string can use all of the JSON control sequences. 

See the [TSON Specification](http://tsonspec) page for more details.

This project contains the source code for that page plus reference implementations.

### C&#35;

The C# library contains simple, efficient and cononical tokenizer and parser and utility classes for TSON.  Notice that there is no object serialization or DOM support in the library.  It is expected that you will use your existing JSON, JSV or XML serialization/DOM functionality from one of the excellent available libraries, for example:

- [JSON.NET](http://james.newtonking.com/json)
- [ServiceStack.Text](https://github.com/ServiceStack/ServiceStack.Text)
- [LINQ to XML](http://msdn.microsoft.com/en-us/library/bb387098.aspx)

#### `Tson`

To use this class, include the `TsonLibrary` project from [NuGet](https://www.nuget.org/packages/TsonLibrary/) and add:

    using TsonLibrary;

The class contains the following methods.

`static bool Validate(string tson)`

Validate that the passed in TSON string is valid.

`static string Format(string tson)`

Format the passed in string using the standard TSON conventions.

`static string ToJson(string tson)`

Convert the passed in TSON to compact JSON.

`static string ToJsv(string tson)`

Convert the passed in TSON to JSV (which is whitespace sensitive).

`public static string ToXml(string tson)`

Convert the passed in TSON to XML.