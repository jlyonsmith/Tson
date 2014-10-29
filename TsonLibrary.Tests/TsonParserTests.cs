using NUnit.Framework;
using System;
using TsonLibrary;
using System.Text;
using System.Globalization;

namespace TsonLibrary.Tests
{
    [TestFixture()]
    public class TsonParserTests
    {
        [Test()]
        public void TestEmpty()
        {
            string tsonText = "";
            string expectedText = "{}";
            TsonNode node = new TsonParser().Parse(tsonText);

            Assert.IsInstanceOf<TsonNode>(node);
            Assert.AreEqual(expectedText, new CompactNodeVisitor(node).ToString());
        }

        [Test()]
        public void TestBadTrailing()
        {
            string tsonText = @"# Comment
{

}
{";
            Assert.Throws<TsonParseException>(() => new TsonParser().Parse(tsonText));
        }

        [Test()]
        public void TestBadTrailingBrace()
        {
            string tsonText = @"{a:1";
            Assert.Throws<TsonParseException>(() => new TsonParser().Parse(tsonText));
        }

        [Test()]
        public void TestEmptyObject()
        {
            string tsonText = @"
{
}";
            string expectedText = @"{}";
            TsonNode node = new TsonParser().Parse(tsonText);

            Assert.IsInstanceOf<TsonObjectNode>(node);
            Assert.AreEqual(expectedText, new CompactNodeVisitor(node).ToString());
        }

        [Test()]
        public void TsetRootObjectBraces()
        {
            string tsonText = @"# A comment
{
    a: 123,
    b: 456
}";
            string expectedText = "{a:123,b:456}";
            TsonNode node = new TsonParser().Parse(tsonText);

            Assert.IsInstanceOf<TsonObjectNode>(node);
            Assert.AreEqual(expectedText, new CompactNodeVisitor(node).ToString());
        }

        [Test()]
        public void TestNoRootObjectBraces()
        {
            string tsonText = @"
a: 123,
b: 456
# Trailing comment";
            string expectedText = "{a:123,b:456}";
            TsonNode node = new TsonParser().Parse(tsonText);

            Assert.IsInstanceOf<TsonObjectNode>(node);
            Assert.AreEqual(expectedText, new CompactNodeVisitor(node).ToString());
        }

        [Test()]
        public void TestBadRootArray()
        {
            string tsonText = @"# A comment
[
    123    ,
    456
]";
            Assert.Throws<TsonParseException>(() => new TsonParser().Parse(tsonText));
        }

        [Test()]
        public void TestNestedObjectsAndArrays()
        {
            string tsonText = @"
{
    m: { 1:a, 2:b, 3:c},
    n: [ 5, { a:1, b:[q, p, r], c:3}, [ ""x"", ""y"", ""z""] ]
}";
            string expectedText = @"{m:{1:a,2:b,3:c},n:[5,{a:1,b:[q,p,r],c:3},[""x"",""y"",""z""]]}";
            TsonNode node = new TsonParser().Parse(tsonText);

            Assert.IsInstanceOf<TsonObjectNode>(node);
            Assert.AreEqual(expectedText, new CompactNodeVisitor(node).ToString());
        }

        [Test()]
        public void TestAllTypes()
        {
            string tsonText = @"
r0: null, # null
s1: abc, # string
s2: ""abc"", # quoted string
s3: """", # empty string
n1: 123, # number
n2: -123, # neg. number
n3: 123.456, # decimal
n4: 123e10, # float with mantissa
n5: 123E-5, # float with neg. mantissa
b1: true, # boolean
b2: false, # boolean
a1: [], # array
o1: {} # object
";
            string expectedText = @"{r0:null,s1:abc,s2:""abc"",s3:"""",n1:123,n2:-123,n3:123.456,n4:1.23E+12,n5:0.00123,b1:true,b2:false,a1:[],o1:{}}";
            TsonNode node = new TsonParser().Parse(tsonText);

            Assert.IsInstanceOf<TsonObjectNode>(node);
            Assert.AreEqual(expectedText, new CompactNodeVisitor(node).ToString());
        }
    }
}

