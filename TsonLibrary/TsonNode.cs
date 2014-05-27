using System;
using System.Collections.Generic;

namespace TsonLibrary
{
    public enum TsonNodeType
    {
        RootObject,
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

    public class TsonObjectNode : TsonNode
    {
        public List<KeyValuePair<TsonStringNode, TsonNode>> KeyValues { get; protected set; }

        public TsonObjectNode(TsonNodeType nodeType = TsonNodeType.Object) : base(nodeType)
        {
            this.KeyValues = new List<KeyValuePair<TsonStringNode, TsonNode>>();
        }

        public TsonObjectNode(List<KeyValuePair<TsonStringNode, TsonNode>> keyValues) : this()
        {
            this.KeyValues = keyValues;
        }
    }

    public class TsonRootObjectNode : TsonObjectNode
    {
        public string PreComment { get; private set; }

        public TsonRootObjectNode() : base(TsonNodeType.RootObject)
        {
        }

        public TsonRootObjectNode(List<KeyValuePair<TsonStringNode, TsonNode>> keyValues) : this()
        {
            this.KeyValues = keyValues;
        }
    }

    public class TsonArrayNode : TsonNode
    {
        public List<TsonNode> Values { get; protected set; }

        public TsonArrayNode() : base(TsonNodeType.Array)
        {
            this.Values = new List<TsonNode>();
        }

        public TsonArrayNode(List<TsonNode> values) : this()
        {
            this.Values = values;
        }
    }

    public abstract class TsonValueNode<T> : TsonNode
    {
        public T Value { get; protected set; }

        public TsonValueNode(TsonNodeType nodeType) : base(nodeType)
        {
        }
    }

    public class TsonBooleanNode : TsonValueNode<Boolean>
    {
        public TsonBooleanNode(Boolean b) : base(TsonNodeType.Boolean)
        {
            this.Value = b;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class TsonNullNode : TsonValueNode<Object>
    {
        public TsonNullNode() : base(TsonNodeType.Null)
        {
            this.Value = null;
        }
    }

    public class TsonNumberNode : TsonValueNode<Double>
    {
        public TsonNumberNode(Double n) : base(TsonNodeType.Number)
        {
            this.Value = n;
        }
    }

    public class TsonStringNode : TsonValueNode<String>
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

