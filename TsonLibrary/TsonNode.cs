using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;

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
        Custom
    }

    public abstract class TsonNode
    {
        public TsonNodeType NodeType 
        { 
            get
            {
                var type = this.GetType();

                if (type.IsGenericType)
                {
                    var genericType = type.GetGenericTypeDefinition();

                    if (genericType == typeof(TsonArrayNode<>))
                        return TsonNodeType.Array;
                    else if (genericType == typeof(TsonValueNode<>))
                    {
                        type = genericType.GetGenericArguments()[0];

                        if (type == typeof(TsonNullNode))
                            return TsonNodeType.Null;
                        else if (type == typeof(TsonNumberNode))
                            return TsonNodeType.Number;
                        if (type == typeof(TsonStringNode))
                            return TsonNodeType.String;
                        else if (type == typeof(TsonBooleanNode))
                            return TsonNodeType.Boolean;
                        else if (type == typeof(TsonNumberNode))
                            return TsonNodeType.Number;
                        else 
                            return TsonNodeType.Custom;
                    }
                    else
                        return TsonNodeType.Object;
                }
                else
                {
                    if (type == typeof(TsonNullNode))
                        return TsonNodeType.Null;
                    else if (type == typeof(TsonNumberNode))
                        return TsonNodeType.Number;
                    if (type == typeof(TsonStringNode))
                        return TsonNodeType.String;
                    else if (type == typeof(TsonBooleanNode))
                        return TsonNodeType.Boolean;
                    else if (type == typeof(TsonNumberNode))
                        return TsonNodeType.Number;
                    else if (type == typeof(TsonArrayNode))
                        return TsonNodeType.Array;
                    else
                        return TsonNodeType.Object;
                }
            }
        }
        public TsonToken Token { get; set; }

        public bool IsObject { get { return NodeType == TsonNodeType.Object; } }
        public bool IsArray { get { return NodeType == TsonNodeType.Array; } }
        public bool IsNumber { get { return NodeType == TsonNodeType.Number; } }
        public bool IsString { get { return NodeType == TsonNodeType.String; } }
        public bool IsBoolean { get { return NodeType == TsonNodeType.Boolean; } }
        public bool IsNull { get { return NodeType == TsonNodeType.Null; } }
        public bool IsCustom { get { return NodeType == TsonNodeType.Custom; } }
    }

    public abstract class TsonValueNode<T> : TsonNode
    {
        public T Value { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            var otherNode = obj as TsonValueNode<T>;

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

    public sealed class TsonBooleanNode : TsonValueNode<Boolean>
    {
        public TsonBooleanNode()
        {
            this.Value = false;
        }

        public TsonBooleanNode(Boolean b)
        {
            this.Value = b;
        }
    }

    public sealed class TsonNullNode : TsonValueNode<Object>
    {
        public TsonNullNode()
        {
            this.Value = null;
        }
    }

    public sealed class TsonNumberNode : TsonValueNode<Double>
    {
        public TsonNumberNode()
        {
            this.Value = 0.0; 
        }

        public TsonNumberNode(Double n) : base()
        {
            this.Value = n;
        }
    }

    public sealed class TsonStringNode : TsonValueNode<String>
    {
        public bool IsQuoted { get; set; }

        public TsonStringNode()
        {
            this.Value = String.Empty;
        }

        public TsonStringNode(String s) : base()
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

    public abstract class TsonArrayNodeBase : TsonNode, IEnumerable
    {
        protected List<TsonNode> values;

        public TsonArrayNodeBase()
        {
            values = new List<TsonNode>();
        }

        public TsonArrayNodeBase(IEnumerable<TsonNode> values)
        {
            this.values = values.ToList();
        }

        public int Count { get { return values.Count; } }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var value in values)
            {
                yield return value;
            }
        }
    }

    public sealed class TsonArrayNode : TsonArrayNodeBase, IEnumerable<TsonNode>
    {
        public TsonArrayNode() : base()
        {
        }

        public TsonArrayNode(IEnumerable<TsonNode> values) : base(values)
        {
        }

        public void Add(TsonNode node)
        {
            values.Add(node);
        }

        public void Add(string s)
        {
            Add(new TsonStringNode(s));
        }

        public void Add(double d)
        {
            Add(new TsonNumberNode(d));
        }

        public void Add(bool b)
        {
            Add(new TsonBooleanNode(b));
        }

        public void Add()
        {
            Add(new TsonNullNode());
        }

        public TsonNode this[int index]
        {
            get
            {
                if (index < values.Count)
                    return values[index];

                do
                {
                    Add(new TsonNullNode());
                }
                while (index >= values.Count);

                return values[index];
            }
            set
            {
                if (index < values.Count)
                {
                    values[index] = value;
                    return;
                }

                while (index > values.Count)
                {
                    Add(new TsonNullNode());
                }

                values.Add(value);
            }
        }


        public IEnumerator<TsonNode> GetEnumerator()
        {
            foreach (var value in values)
            {
                yield return value;
            }
        }
    }

    public sealed class TsonArrayNode<T> : TsonArrayNodeBase, IEnumerable<T> where T : TsonNode, new()
    {
        public TsonArrayNode() : base()
        {
        }

        public TsonArrayNode(IEnumerable<T> values)
        {
            this.values = values.Cast<TsonNode>().ToList();
        }

        public void Add(T node)
        {
            this.values.Add((TsonNode)node);
        }

        public TsonNode this[int index]
        {
            get
            {
                if (index < values.Count)
                    return values[index];

                do
                {
                    Add(new T());
                }
                while (index >= values.Count);

                return values[index];
            }
            set
            {
                if (index < values.Count)
                {
                    values[index] = value;
                    return;
                }

                while (index > values.Count)
                {
                    Add(new T());
                }

                values.Add(value);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var value in values)
            {
                yield return (T)value;
            }
        }
    }

    public abstract class TsonObjectNodeBase : TsonNode, IEnumerable<KeyValuePair<TsonStringNode, TsonNode>>
    {
        public abstract IEnumerator<KeyValuePair<TsonStringNode, TsonNode>> GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        // TODO: Override Equals() and GetHashCode()
    }

    public sealed class TsonObjectNode : TsonObjectNodeBase
    {
        private List<KeyValuePair<TsonStringNode, TsonNode>> keyValues;

        public TsonObjectNode() : base()
        {
            keyValues = new List<KeyValuePair<TsonStringNode, TsonNode>>();
        }

        public TsonObjectNode(IEnumerable<KeyValuePair<string, TsonNode>> keyValues)
        {
            throw new NotImplementedException();
        }

        public void Add(string key, double n)
        {
            this.Add(key, new TsonNumberNode(n));
        }

        public void Add(string key, string s)
        {
            this.Add(key, new TsonStringNode(s));
        }

        public void Add(string key, bool b)
        {
            this.Add(key, new TsonBooleanNode(b));
        }

        public void Add(string key)
        {
            this.Add(key, new TsonNullNode());
        }

        public void Add(string key, TsonNode valueNode)
        {
            this.Add(new TsonStringNode(key), valueNode);
        }

        public void Add(TsonStringNode keyNode, TsonNode valueNode)
        {
            this.keyValues.Add(new KeyValuePair<TsonStringNode, TsonNode>(keyNode, valueNode));
        }

        public override IEnumerator<KeyValuePair<TsonStringNode, TsonNode>> GetEnumerator()
        {
            foreach (var keyValue in keyValues)
            {
                yield return keyValue;
            }
        }
    }

    public abstract class TsonTypedObjectNode : TsonObjectNodeBase
    {
        IEnumerable<PropertyInfo> propInfos;

        public TsonTypedObjectNode()
        {
            propInfos = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(pi => typeof(TsonNode).IsAssignableFrom(pi.PropertyType));
        }

        public override IEnumerator<KeyValuePair<TsonStringNode, TsonNode>> GetEnumerator()
        {
            foreach (var propInfo in propInfos)
            {
                yield return new KeyValuePair<TsonStringNode, TsonNode>(
                    new TsonStringNode(propInfo.Name), 
                    (TsonNode)propInfo.GetValue(this));
            }
        }

        public bool HasInvalidNulls()
        {
            foreach (var propInfo in propInfos)
            {
                var attr = propInfo.GetCustomAttribute<TsonNotNullAttribute>();

                if (attr != null && propInfo.GetValue(this) == null)
                    return true;
            }

            return false;
        }
    }
}

