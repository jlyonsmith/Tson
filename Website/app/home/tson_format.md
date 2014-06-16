## TSON Format

### Overview

The TSON (Typeable Simple Object Notation) is based on the [JSON](http://www.json.org/) and [JSV](http://mono.servicestack.net/mythz_blog/?p=176) data formats.  It is intended to be a _storage_ and not a _transmission_ format, and one that is editable by humans.  As such it includes the following primary differences which benefit humans vs. computers:

- Double quotes (" ") are optional, and can be omitted for most simple alphanumeric strings.
- When quotes are used, everything within them, including newlines, is part of the string.  Additionally, you can use any of the JSON control sequences beginning with `\`.
- You can include comments in the data using the hash (#) symbol.
- TSON data always contains an _object_ at the root.  The root curly brackets ({ }) are optional _only for this object_.

TSON only supports 3 data types:

- Strings (which can be quoted or unquoted)
- Objects
- Arrays

Comments are not considered to be data.  Because TSON supports unquoted strings, a string can represent a number, boolean and null as defined by JSON.  If you stick to these data types you can easily convert TSON to JSON.  If you don't need JSON compatability, you can define other custom data types of your choosing.

### Example

Let's start with an example. Here is simple TSON data file:

    # A simple TSON data file, convertible to JSON
    name: TSON
    description: A data storage format
    url: "http://tsonspec.org"
    created: 2014-06-01

Here is one that is a bit more complex that shows all of the main TSON features:

    # TSON data file example that 
    # is still convertible to JSON
    a: some data,
    b: some more data,
    
    # Blank lines are fine
    
    "c#1": 10, # Keys can be quoted too if needed
    
    e: # This will be an object
    {
    	x: true,
    	y: false
    },
    
    f: # Now let's try an array
    [
    	10, 20, 30
    ],
    
    g: "Finally, let's create a 
    long string that 
    has line breaks in 
    it for fun."
    
    # That's it.  Try cutting and pasting this in to
    # the formatters and converters to see what 
    # happens!
    
### Syntax

TSON syntax is extremely simple.  Data in both JSON and JSV formats can be represented in TSON, but TSON is superset in that it contains support for comments.

#### Unquoted String

An unquoted string is defined as follows:

![UnquotedString](images/UnquotedString.png)

Unquoted strings may include any characters that are not in the list of TSON "punctuation" characters:

    " , : [ ] { } #
    
Whitespace at the beginning and end of an unquoted string is trimmed off and is **not** considered part of the string.  For example:

    a:   A string in space    # with a comment
    
The value of `a` is simply `A string in space`.  If you need the whitespace, quote the string.

JSON control sequences are ignored in an unquoted string.

#### Quoted String

A quoted string is defined as follows:

![QuotedString](images/QuotedString.png)

Within a quoted string any text is valid except `\` and `"`.  A double quote (`"`) may be included by using the standard JSON control sequence `\"`.  See the diagram for the other allowed control sequences.  Newlines, carriage returns, tabs and forward slashes (`/`) can also be included in the string just by typing them, as your editor allows.  An empty string is represented by  `""`.

#### String

A string is either quoted or unquoted:

![String](images/String.png)

#### Array

An array is defined as follows:

![Array](images/Array.png)

#### Object

Finally, an object is defined as follows:

![Object](images/Object.png)

The exception is the root object, where the curly brackets are optional.

### Implementations

#### C&#35;

There is a reference implementation in C# along with this website, web service and additional sample and test code at [https://github.com/jlyonsmith/tson](https://github.com/jlyonsmith/tson).
