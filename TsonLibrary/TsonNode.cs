using System;
using System.Collections.Generic;

namespace TsonLibrary
{
    public enum TsonNodeType
    {
        Object,
        Array,
        Number,
        String,
        Boolean,
        Null,
        Comment
    }

    public class TsonNode
    {
        protected TsonNode(TsonNodeType nodeType)
        {
            this.NodeType = nodeType;
        }

        public TsonNodeType NodeType { get; private set; }
        public TsonToken Token { get; protected set; }
        public string Comment { get; protected set; }

        public bool IsObject { get { return NodeType == TsonNodeType.Object; } }
        public bool IsArray { get { return NodeType == TsonNodeType.Array; } }
        public bool IsNumber { get { return NodeType == TsonNodeType.Number; } }
        public bool IsString { get { return NodeType == TsonNodeType.String; } }
        public bool IsBoolean { get { return NodeType == TsonNodeType.Boolean; } }
        public bool IsNull { get { return NodeType == TsonNodeType.Null; } }
    }

    public class TsonKeyedNodeList : List<KeyValuePair<TsonStringNode, TsonNode>>
    {
        public void Add(string key, double n)
        {
            this.Add(new KeyValuePair<TsonStringNode, TsonNode>(
                new TsonStringNode(key),
                new TsonNumberNode(n)));
        }

        public void Add(string key, string s)
        {
            this.Add(new KeyValuePair<TsonStringNode, TsonNode>(
                new TsonStringNode(key),
                new TsonStringNode(s)));
        }

        public void Add(string key, bool b)
        {
            this.Add(new KeyValuePair<TsonStringNode, TsonNode>(
                new TsonStringNode(key),
                new TsonBooleanNode(b)));
        }

        public void Add(string key)
        {
            this.Add(new KeyValuePair<TsonStringNode, TsonNode>(
                new TsonStringNode(key),
                new TsonNullNode()));
        }

        public void Add(string key, TsonNode node)
        {
            this.Add(new KeyValuePair<TsonStringNode, TsonNode>(
                new TsonStringNode(key), node));
        }
    }

    public class TsonNodeList : List<TsonNode>
    {
        public void Add(double n)
        {
            this.Add(new TsonNumberNode(n));
        }

        public void Add(string s)
        {
            this.Add(new TsonStringNode(s));
        }

        public void Add(bool b)
        {
            this.Add(new TsonBooleanNode(b));
        }

        public void Add()
        {
            this.Add(new TsonNullNode());
        }
    }

    public class TsonObjectNode : TsonNode
    {
        public TsonKeyedNodeList KeyValues { get; protected set; }

        public TsonObjectNode() : base(TsonNodeType.Object)
        {
            this.KeyValues = new TsonKeyedNodeList();
        }

        public TsonObjectNode(TsonKeyedNodeList keyValues) : this()
        {
            this.KeyValues = keyValues;
        }
    }

    public class TsonRootObjectNode : TsonObjectNode
    {
        public string PreComment { get; private set; }

        public TsonRootObjectNode() : base()
        {
        }

        public TsonRootObjectNode(TsonKeyedNodeList keyValues) : this()
        {
            this.KeyValues = keyValues;
        }
    }

    public class TsonArrayNode : TsonNode
    {
        public TsonNodeList Values { get; protected set; }

        public TsonArrayNode() : base(TsonNodeType.Array)
        {
            this.Values = new TsonNodeList();
        }

        public TsonArrayNode(TsonNodeList values) : this()
        {
            this.Values = values;
        }

        public void Add(TsonStringNode node)
        {
            this.Values.Add(node);
        }

        public void Add(TsonNumberNode node)
        {
            this.Values.Add(node);
        }

        public void Add(TsonBooleanNode node)
        {
            this.Values.Add(node);
        }

        public void Add(TsonNullNode node)
        {
            this.Values.Add(node);
        }
    }

    public interface ITsonValueNode<T>
    {
        T Value { get; set; }
    }

    public abstract class TsonValueNode<TData, TNode> : TsonNode, ITsonValueNode<TData> where TNode: class,ITsonValueNode<TData>
    {
        public TData Value { get; set; }

        public TsonValueNode(TsonNodeType nodeType) : base(nodeType)
        {
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            var otherNode = obj as TNode;

            if (otherNode == null)
                return false;

            return this.Value.Equals(otherNode.Value);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", this.GetType().FullName, Value.ToString());
        }
    }

    public class TsonBooleanNode : TsonValueNode<Boolean, TsonBooleanNode>
    {
        public TsonBooleanNode(Boolean b) : base(TsonNodeType.Boolean)
        {
            this.Value = b;
        }
    }

    public class TsonNullNode : TsonValueNode<Object, TsonNullNode>
    {
        public TsonNullNode() : base(TsonNodeType.Null)
        {
            this.Value = null;
        }
    }

    public class TsonNumberNode : TsonValueNode<Double, TsonNumberNode>
    {
        public TsonNumberNode(Double n) : base(TsonNodeType.Number)
        {
            this.Value = n;
        }
    }

    public class TsonStringNode : TsonValueNode<String, TsonStringNode>
    {
        public bool IsQuoted { get; set; }

        public TsonStringNode(String s) : base(TsonNodeType.String)
        {
            if (s.Length > 0 && s[0] == '"')
            {
                this.Value = s.Substring(1, s.Length - 2);
                this.IsQuoted = true;
            }
            else
            {
                this.Value = s;
                this.IsQuoted = false;
            }
        }
    }
}

