using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace TsonLibrary
{
    public class ToTypedTsonNodeVisitor<T> : TsonNodeVisitor where T : TsonTypedObjectNode
    {
        TsonObjectNode rootNode;
        PropertyContext owner;

        protected T TargetObject { get; set; }

        public ToTypedTsonNodeVisitor(TsonObjectNode rootNode)
        {
            this.rootNode = rootNode;
        }

        public T ToTypedObjectNode()
        {
            var propInfo = this.GetType().GetProperty("TargetObject", BindingFlags.NonPublic | BindingFlags.Instance);

            this.owner = new PropertyContext(this, propInfo);

            Visit(rootNode);

            return TargetObject;
        }

        private void UpdateOwnerObject(TsonNode node)
        {
            if (owner.HasIndex)
            {
                owner.SetItemValue(node);
            }
            else
            {
                owner.SetPropertyValue(node);
            }
        }

        protected override TsonNode VisitRootObject(TsonObjectNodeBase node)
        {
            return VisitObject((TsonObjectNode)node);
        }

        protected override TsonNode VisitObject(TsonObjectNodeBase node)
        {
            if (!(node is TsonObjectNode))
                throw new NotSupportedException();

            object obj = null;
            Type type = null;

            if (owner.HasIndex)
            {
                if (owner.ItemType == typeof(TsonObjectNode))
                {
                    owner.SetItemValue(node);
                    return node;
                }
                else
                {
                    type = owner.ItemType;
                    obj = Activator.CreateInstance(type);
                    owner.SetItemValue(obj);
                }
            }
            else
            {
                if (owner.PropertyType == typeof(TsonObjectNode))
                {
                    owner.SetPropertyValue(node);
                    return node;
                }
                else
                {
                    type = owner.PropertyType;
                    obj = Activator.CreateInstance(type);
                    owner.SetPropertyValue(obj);
                }
            }

            var savedOwner = owner;

            foreach (var childNode in node)
            {
                var propInfo = type.GetProperty(childNode.Key.Value);

                if (propInfo == null)
                    continue;

                owner = new PropertyContext(obj, propInfo);

                Visit(childNode.Value);
            }

            owner = savedOwner;

            if (((TsonTypedObjectNode)obj).HasInvalidNulls())
                throw new TsonFormatException();

            return node;
        }

        protected override TsonNode VisitArray(TsonArrayNodeBase node)
        {
            if (!(node is TsonArrayNode))
                throw new NotSupportedException();

            if (owner.HasIndex)
            {
                if (owner.ItemType == typeof(TsonArrayNode))
                {
                    owner.SetItemValue(node);
                    return node;
                }
                else
                {
                    owner.SetItemValue(Activator.CreateInstance(owner.ItemType));
                }
            }
            else
            {
                if (owner.PropertyType == typeof(TsonArrayNode))
                {
                    owner.SetPropertyValue(node);
                    return node;
                }
                else
                {
                    owner.SetPropertyValue(Activator.CreateInstance(owner.PropertyType));
                }
            }

            var savedOwner = owner;

            int i = 0;

            foreach (var subNode in node)
            {
                if (!typeof(TsonArrayNodeBase).IsAssignableFrom(savedOwner.PropertyType))
                    continue;

                owner = new PropertyContext(savedOwner.Instance, savedOwner.PropertyInfo, i);

                Visit((TsonNode)subNode);

                i++;
            }

            owner = savedOwner;

            return node;
        }

        protected override TsonNode VisitNumber(TsonNumberNode node)
        {
            UpdateOwnerObject(node);

            return node;
        }

        protected override TsonNode VisitString(TsonStringNode node)
        {
            UpdateOwnerObject(node);

            return node;
        }

        protected override TsonNode VisitBoolean(TsonBooleanNode node)
        {
            UpdateOwnerObject(node);

            return node;
        }
    }
}

