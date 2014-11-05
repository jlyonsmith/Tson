## TSON: Typeable Simple Object Notation

### Rational

TSON is a less rigid form of JSON to make it as easy for humans to type as possible.  It makes a great data storage format for any type of data that humans will edit.  It supports:

- Comments using the simplest comment convention around; the # symbol
- All strings can be left unquoted, provided they don't contain the characters `",:[]{}`
- Real newlines and tabs in strings are perfectly OK.
- If your string needs any of the 7 reserved TSON characters, or is empty, just put double quotes around it ("...")
- In addition, double quoted string can use all of the JSON control sequences. 

See the [TSON Specification](http://tsonspec) page for more details.

This project contains the source code for that page plus reference implementations.

### C&#35;

The C# library contains simple, efficient and cononical tokenizer and parser and utility classes for TSON.  It is expected that you will use your existing JSON, JSV or XML serialization/DOM functionality from one of the excellent available libraries, for example:

- [JSON.NET](http://james.newtonking.com/json)
- [ServiceStack.Text](https://github.com/ServiceStack/ServiceStack.Text)
- [LINQ to XML](http://msdn.microsoft.com/en-us/library/bb387098.aspx)

As of build 1.0.11105.1 TsonLibrary now provides object serialization and deserialization. See below for details.


#### `Tson`

To use this class, include the `TsonLibrary` project from [NuGet](https://www.nuget.org/packages/TsonLibrary/) and add:

    using TsonLibrary;

The class contains the following methods:

#### `static bool Validate(string tson)`

Validate that the passed in TSON string is valid.

#### `static string Format(string tson)`

Format the passed in string using the standard TSON conventions.

#### `static string ToJson(string tson)`

Convert the passed in TSON to compact JSON.

#### `static string ToJsv(string tson)`

Convert the passed in TSON to JSV (which is whitespace sensitive).

#### `public static string ToXml(string tson)`

Convert the passed in TSON to XML.

#### `public static T ToObjectNode<T>(string tson)`

Parse the passed in TSON into a strongly type `TsonTypedObjectNode` object tree.

#### `public static TsonObjectNode ToObjectNode(string tson)`

Parse the passed in TSON into a loosely typed `TsonObjectNode` object tree.

### Serialization/Deserialization

The serialization classes take a slightly different approach to other C# serialization classes.  Here are the design goals:

- TSON based file formats should be able to be defined using [POCO's](http://en.wikipedia.org/wiki/Plain_Old_CLR_Object); no special _schema_ syntax

- There should be support for both strongly typed and loosely typed fields, e.g. a field could be an arbitrary TSON object, or it could be a specific one with required fields.

- All fields should be `TsonNode` derived, so that they can retain references to the `TsonToken` that created them.  In this way error messages can point to exact locations in the TSON data.

- Extra fields present in the data should be ignored.

To create a TSON serializable C# class you must derive the class from `TsonTypedObjectNode`.  Then create a `public property` for each field in object using one of:

- `TsonObjectNode`
- `TsonArrayNode`
- `TsonArrayNode<T>` where T is any of these types
- `TsonStringNode`
- `TsonNumberNode`
- `TsonBooleanNode`
- `TsonNullNode`
- ... or a `TsonTypedObjectNode` derived class

Here is an example:

```
class Data : TsonTypedObjectNode
{
    [TsonNotNull]
    public TsonNumberNode NumNode { get; set; }
    [TsonNotNull]
    public TsonStringNode StringNode { get; set; }
    [TsonNotNull]
    public TsonBooleanNode BoolNode { get; set; }
    [TsonNotNull]
    public TsonObjectNode ObjectNode { get; set; }
    [TsonNotNull]
    public TsonArrayNode ArrayNode { get; set; }
    public CustomData CustomData { get; set; }
    public TsonArrayNode<TsonStringNode> StringNodeList { get; set; }
    public TsonArrayNode<TsonNumberNode> NumberNodeList { get; set; }
    public TsonArrayNode<CustomData> CustomDataList { get; set; }
    public TsonArrayNode<TsonObjectNode> ObjectNodeList { get; set; }
}

class CustomData : TsonTypedObjectNode
{
    public TsonStringNode Thing1 { get; set; }
    public TsonNumberNode Thing2 { get; set; }
}
```
Given the TSON data in the file `data.tson`:

```
NumNode: 10,
StringNode: abc,
BoolNode: true,
ObjectNode: { a: 1, b: 2 },
ArrayNode: [ 1, 2 ],
CustomData: { Thing1: a, Thing2: 2 },
StringNodeList: [ a, b, c ],
NumberNodeList: [ 1, 2, 3 ],
CustomDataList: [ { Thing1: a, Thing2: 1 }, { Thing1: b, Thing2: 2 } ]
```
You could read and validate this data using just the code:

```
Tson.ToObjectNode<Data>(File.ReadAllText("data.tson"))
```
If a `TsonFormatException` or `TsonParseException` is generated, it will indicate where in the file the error occurs.

Note the use of the attribute `TsonNotNull` to indicate fields that must be present in the data.

 