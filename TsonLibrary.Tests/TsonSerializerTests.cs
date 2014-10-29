using NUnit.Framework;
using System;
using TsonLibrary;
using System.Collections.Generic;

namespace TsonLibrary.Tests
{
    [TestFixture()]
    public class TsonSerializerTests
    {
        class Data
        {
            public TsonNumberNode NumNode { get; set; }
            public TsonStringNode StringNode { get; set; }
            public TsonBooleanNode BoolNode { get; set; }
            public TsonObjectNode ObjectNode { get; set; }
            public TsonArrayNode ArrayNode { get; set; }
            public CustomData CustomData { get; set; }
            public List<TsonStringNode> StringNodeList { get; set; }
            public List<TsonNumberNode> NumberNodeList { get; set; }
            public List<CustomData> CustomDataList { get; set; }
        }

        class CustomData
        {
            public TsonStringNode Thing1 { get; set; }
            public TsonNumberNode Thing2 { get; set; }

            public override bool Equals(object obj)
            {
                if (obj == null)
                    return false;

                if (Object.ReferenceEquals(this, obj))
                    return true;

                var other = obj as CustomData;

                if (other == null)
                    return false;

                return Thing1.Value == other.Thing1.Value && Thing2.Value == other.Thing2.Value;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        [Test()]
        public void TestSerialize()
        {
            var data = new Data
            {
                NumNode = new TsonNumberNode(10.0),
                StringNode = new TsonStringNode("abc"),
                BoolNode = new TsonBooleanNode(true),
                ObjectNode = new TsonObjectNode(new TsonKeyedNodeList
                { 
                    { "A", 1.23 },
                    { "B", 2 }
                }),
                ArrayNode = new TsonArrayNode(new TsonNodeList { 1.0, 2.0 }),
                CustomData = new CustomData() { Thing1 = new TsonStringNode("xyz"), Thing2 = new TsonNumberNode(3.14) },
                StringNodeList = new List<TsonStringNode> 
                {
                    new TsonStringNode("John"),
                    new TsonStringNode("Jamey")
                },
                NumberNodeList = new List<TsonNumberNode>
                {
                    new TsonNumberNode(40),
                    new TsonNumberNode(10)
                },
                CustomDataList = new List<CustomData> 
                {
                    new CustomData() { Thing1 = new TsonStringNode("pi"), Thing2 = new TsonNumberNode(3.14) },
                    new CustomData() { Thing1 = new TsonStringNode("e"), Thing2 = new TsonNumberNode(2.72) }
                }
            };
            string tson = TsonSerializer.Serialize<Data>(data);

            Assert.AreEqual(
                "{NumNode:10,StringNode:abc,BoolNode:true,ObjectNode:{A:1.23,B:2},ArrayNode:[1,2],CustomData:{Thing1:xyz,Thing2:3.14},"+
                "StringNodeList:[John,Jamey],NumberNodeList:[40,10],CustomDataList:[{Thing1:pi,Thing2:3.14},{Thing1:e,Thing2:2.72}]}", tson);
        }

        [Test()]
        public void TestDeserializeFull()
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
            var data = TsonSerializer.Deserialize<Data>(tson);

            Assert.AreEqual(new TsonNumberNode(10), data.NumNode);
            Assert.AreEqual(new TsonStringNode("abc"), data.StringNode);
            Assert.AreEqual(new TsonBooleanNode(true), data.BoolNode);
            CollectionAssert.AreEqual(new TsonKeyedNodeList { { "a", 1.0 }, { "b", 2.0} } , data.ObjectNode.KeyValues);
            CollectionAssert.AreEqual(new TsonNodeList { 1.0, 2.0 } , data.ArrayNode.Values);
            Assert.AreEqual(new CustomData() { Thing1 = new TsonStringNode("a"), Thing2 = new TsonNumberNode(2) }, data.CustomData);
            CollectionAssert.AreEqual(new List<TsonStringNode> 
            { 
                new TsonStringNode("a"), 
                new TsonStringNode("b"), 
                new TsonStringNode("c"), 
            }, data.StringNodeList);
            CollectionAssert.AreEqual(new List<TsonNumberNode> 
            { 
                new TsonNumberNode(1), 
                new TsonNumberNode(2), 
                new TsonNumberNode(3), 
            }, data.NumberNodeList);
            CollectionAssert.AreEqual(new List<CustomData> 
            { 
                new CustomData() { Thing1 = new TsonStringNode("a"), Thing2 = new TsonNumberNode(1) }, 
                new CustomData() { Thing1 = new TsonStringNode("b"), Thing2 = new TsonNumberNode(2) }, 
            }, data.CustomDataList);
        }

        [Test()]
        public void TestDeserializePartial()
        {
            var tson = @"NumNode: 10,
StringNode: abc,
ObjectNode: { a: 1, b: 2 },
ArrayNode: [ 1, 2 ]
";
            var data = TsonSerializer.Deserialize<Data>(tson);

            Assert.AreEqual(new TsonNumberNode(10), data.NumNode);
            Assert.AreEqual(new TsonStringNode("abc"), data.StringNode);
            CollectionAssert.AreEqual(new TsonKeyedNodeList { { "a", 1.0 }, { "b", 2.0} } , data.ObjectNode.KeyValues);
            CollectionAssert.AreEqual(new TsonNodeList { 1.0, 2.0 } , data.ArrayNode.Values);
        }
    }
}

