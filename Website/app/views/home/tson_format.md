## TSON Format

### Overview

TSON (Typeable Simple Object Notation) is derived from the [JSON](http://www.json.org/) and [JSV](http://mono.servicestack.net/mythz_blog/?p=176) data formats.  It is primarily intended to be a data storage format which is editable by humans, the word _typeable_ implying that it is easier to manually edit.  To support this, it includes the following primary differences to both JSON and JSV formats:

- You can include comments in the data using the hash (#) symbol.
- Double quotes (" ") are optional, and can be omitted for most simple alphanumeric strings, except an empty string which is two double quotes together ("").
- Strings can span multiple lines, whether quoted or not.
- When quotes are _not_ used, newlines, tabs and spaces at the ends of the string are trimmed away.
- When quotes _are_ used all spaces, newlines and tabs are part of the string. 
- Quoted strings can use any of the JSON control sequences beginning with `\`.
- TSON data always contains an _object_ at the root.  Therefore the curly brackets ({ }) are _optional_ for this object.

TSON only validates 3 data types:

- Strings (which can be quoted or unquoted)
- Objects
- Arrays

Because TSON supports unquoted strings, a string can _syntactically_ represent JSON numbers, boolean and null's.  If you stick to these data types you can easily convert TSON to JSON.  If you don't need JSON compatability, you can define other custom data types of your choosing.  

Comments are not generally considered to be part of the data, but they are easy to parse out and treat as such if you wish.

### Example

Let's start with an example. Here is simple TSON data file:

    # A simple TSON data file, convertible to JSON
    name: TSON,
    description: A data storage format,
    url: "http://tsonspec.org",
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
    
    g: "Let's create a 
    long string that 
    has line breaks in 
    it for fun.",
    
    h: And this
    works 
    just fine 
    as well!
    
    # That's it.  Try cutting and pasting this in to
    # the formatters and converters to see what 
    # happens!
    
### Syntax

TSON has a very simple syntax.  Data in both JSON and JSV formats are valid TSON, but TSON is superset in that it contains support for comments and unquoted strings.

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

Within a quoted string any text is valid except backslash (`\`) and double quote (`"`).  Double quotes may be included by using the standard JSON control sequence `\"`.  See the diagram for the other allowed control sequences.  Newlines, carriage returns, tabs and forward slashes (`/`) can also be included in the string just by typing them, as your editor allows.  An empty string is represented by  `""`.

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

There is a reference implementation of TSON in C# at [https://github.com/jlyonsmith/tson](https://github.com/jlyonsmith/tson).  It includes the code for this website, web service and additional sample and test code .  TsonLibrary also available on [NuGet](https://www.nuget.org/packages/TsonLibrary/).
