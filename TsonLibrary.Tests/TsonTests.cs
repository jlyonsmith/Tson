using NUnit.Framework;
using System;
using TsonLibrary;
using System.Collections.Generic;

namespace TsonLibrary.Tests
{
    [TestFixture()]
    public class TsonTests
    {
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
        }

        class CustomData : TsonTypedObjectNode
        {
            public TsonStringNode Thing1 { get; set; }
            public TsonNumberNode Thing2 { get; set; }
        }

        [Test()]
        public void TestFormat()
        {
            var data = new TsonObjectNode
            {
                { "A", 1 }
            };

            string tson = Tson.Format(data, TsonFormatStyle.Compact);

            Assert.AreEqual("{A:1}", tson);
        }

        [Test()]
        public void TestTypedFormat()
        {
            var data = new Data
            {
                NumNode = new TsonNumberNode(10.0),
                StringNode = new TsonStringNode("abc"),
                BoolNode = new TsonBooleanNode(true),
                ObjectNode = new TsonObjectNode
                { 
                    { "A", 1.23 },
                    { "B", 2 }
                },
                ArrayNode = new TsonArrayNode { 1.0, 2.0 },
                CustomData = new CustomData() { Thing1 = new TsonStringNode("xyz"), Thing2 = new TsonNumberNode(3.14) },
                StringNodeList = new TsonArrayNode<TsonStringNode> 
                {
                    new TsonStringNode("John"),
                    new TsonStringNode("Jamey")
                },
                NumberNodeList = new TsonArrayNode<TsonNumberNode>
                {
                    new TsonNumberNode(40),
                    new TsonNumberNode(10)
                },
                CustomDataList = new TsonArrayNode<CustomData> 
                {
                    new CustomData() { Thing1 = new TsonStringNode("pi"), Thing2 = new TsonNumberNode(3.14) },
                    new CustomData() { Thing1 = new TsonStringNode("e"), Thing2 = new TsonNumberNode(2.72) }
                }
            };

            string tson = Tson.Format(data, TsonFormatStyle.Compact);

            Assert.AreEqual(
                "{NumNode:10,StringNode:abc,BoolNode:true,ObjectNode:{A:1.23,B:2},ArrayNode:[1,2],CustomData:{Thing1:xyz,Thing2:3.14},"+
                "StringNodeList:[John,Jamey],NumberNodeList:[40,10],CustomDataList:[{Thing1:pi,Thing2:3.14},{Thing1:e,Thing2:2.72}]}", tson);
        }

        [Test()]
        public void TestFullTypeMapping()
        {
            // Fill in every field in the test class

            var tson = @"
NumNode: 10,
StringNode: abc,
BoolNode: true,
ObjectNode: { a: 1, b: 2 },
ArrayNode: [ 1, 2 ],
CustomData: { Thing1: a, Thing2: 2 },
StringNodeList: [ a, b, c ],
NumberNodeList: [ 1, 2, 3 ],
CustomDataList: [ { Thing1: a, Thing2: 1 }, { Thing1: b, Thing2: 2 } ]
";
            var data = Tson.ToObjectNode<Data>(tson);

            Assert.AreEqual(new TsonNumberNode(10), data.NumNode);
            Assert.AreEqual(new TsonStringNode("abc"), data.StringNode);
            Assert.AreEqual(new TsonBooleanNode(true), data.BoolNode);
            CollectionAssert.AreEqual(new TsonObjectNode { { "a", 1.0 }, { "b", 2.0} } , data.ObjectNode);
            CollectionAssert.AreEqual(new TsonArrayNode { 1.0, 2.0 } , data.ArrayNode);
            Assert.AreEqual(new CustomData() { Thing1 = new TsonStringNode("a"), Thing2 = new TsonNumberNode(2) }, data.CustomData);
            CollectionAssert.AreEqual(new TsonArrayNode<TsonStringNode> 
            { 
                new TsonStringNode("a"), 
                new TsonStringNode("b"), 
                new TsonStringNode("c"), 
            }, data.StringNodeList);
            CollectionAssert.AreEqual(new TsonArrayNode<TsonNumberNode> 
            { 
                new TsonNumberNode(1), 
                new TsonNumberNode(2), 
                new TsonNumberNode(3), 
            }, data.NumberNodeList);
            CollectionAssert.AreEqual(new TsonArrayNode<CustomData> 
            { 
                new CustomData() { Thing1 = new TsonStringNode("a"), Thing2 = new TsonNumberNode(1) }, 
                new CustomData() { Thing1 = new TsonStringNode("b"), Thing2 = new TsonNumberNode(2) }, 
            }, data.CustomDataList);
        }

        [Test()]
        public void TestPartialTypeMapping()
        {
            var tson = @"NumNode: 10,
StringNode: abc,
BoolNode: true,
ObjectNode: { a: 1, b: 2 },
ArrayNode: [ 1, 2 ]
";
            var data = Tson.ToObjectNode<Data>(tson);

            Assert.AreEqual(new TsonNumberNode(10), data.NumNode);
            Assert.AreEqual(new TsonStringNode("abc"), data.StringNode);
            Assert.AreEqual(new TsonBooleanNode(true), data.BoolNode);
            CollectionAssert.AreEqual(new TsonObjectNode { { "a", 1.0 }, { "b", 2.0} } , data.ObjectNode);
            CollectionAssert.AreEqual(new TsonArrayNode { 1.0, 2.0 } , data.ArrayNode);
        }

        [Test()]
        public void TestTypeMappingWithBadNull()
        {
            var tson = @"";

            Assert.Throws<TsonFormatException>(() => Tson.ToObjectNode<Data>(tson));
        }
    }
}

