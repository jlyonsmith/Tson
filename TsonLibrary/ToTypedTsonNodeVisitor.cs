using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace TsonLibrary
{
    public class ToTypedTsonNodeVisitor<T> : TsonNodeVisitor where T : TsonTypedObjectNode
    {
        TsonObjectNode rootNode;
        NodePropertyInstance targetNodeProperty;

        protected T TargetObject { get; set; }

        public ToTypedTsonNodeVisitor(TsonObjectNode rootNode)
        {
            this.rootNode = rootNode;
        }

        public T ToTypedObjectNode()
        {
            var propInfo = this.GetType().GetProperty("TargetObject", BindingFlags.NonPublic | BindingFlags.Instance);

            this.targetNodeProperty = new NodePropertyInstance(this, propInfo);

            Visit(rootNode);

            return TargetObject;
        }

        private void UpdateTargetNodeProperty(TsonNode node)
        {
            if (targetNodeProperty.HasIndex)
            {
                targetNodeProperty.SetItemValue(node);
            }
            else
            {
                targetNodeProperty.SetPropertyValue(node);
            }
        }

        protected override TsonNode VisitRootObject(TsonObjectNodeBase node)
        {
            return VisitObject((TsonObjectNode)node);
        }

        protected override TsonNode VisitObject(TsonObjectNodeBase node)
        {
            TsonNode newNode = null;
            Type type = null;

            if (targetNodeProperty.HasIndex)
            {
                if (targetNodeProperty.ItemType == typeof(TsonObjectNode))
                {
                    targetNodeProperty.SetItemValue(node);
                    return node;
                }
                else
                {
                    type = targetNodeProperty.ItemType;
                    newNode = (TsonNode)Activator.CreateInstance(type);
                    newNode.Token = node.Token;
                    targetNodeProperty.SetItemValue(newNode);
                }
            }
            else
            {
                if (targetNodeProperty.PropertyType == typeof(TsonObjectNode))
                {
                    targetNodeProperty.SetPropertyValue(node);
                    return node;
                }
                else
                {
                    type = targetNodeProperty.PropertyType;
                    newNode = (TsonNode)Activator.CreateInstance(type);
                    newNode.Token = node.Token;
                    targetNodeProperty.SetPropertyValue(newNode);
                }
            }

            var lastTargetNodeProperty = targetNodeProperty;

            foreach (var childNode in node)
            {
                var propInfo = type.GetProperty(childNode.Key.Value);

                if (propInfo == null || !typeof(TsonNode).IsAssignableFrom(propInfo.PropertyType))
                    continue;

                targetNodeProperty = new NodePropertyInstance(newNode, propInfo);

                Visit(childNode.Value);
            }

            targetNodeProperty = lastTargetNodeProperty;

            var typedNode = newNode as TsonTypedObjectNode;

            if (typedNode != null)
            {
                var propInfo = typedNode.FirstInvalidNull();

                if (propInfo != null)
                    throw new TsonFormatException(node.Token, String.Format("Property '{0}' should not be null", propInfo.Name));
            }

            return node;
        }

        protected override TsonNode VisitArray(TsonArrayNodeBase node)
        {
            if (targetNodeProperty.HasIndex)
            {
                if (targetNodeProperty.ItemType == typeof(TsonArrayNode))
                {
                    targetNodeProperty.SetItemValue(node);
                    return node;
                }
                else
                {
                    var newNode = (TsonArrayNodeBase)Activator.CreateInstance(targetNodeProperty.ItemType);

                    newNode.Token = node.Token;
                    targetNodeProperty.SetItemValue(newNode);
                }
            }
            else
            {
                if (targetNodeProperty.PropertyType == typeof(TsonArrayNode))
                {
                    targetNodeProperty.SetPropertyValue(node);
                    return node;
                }
                else
                {
                    var newNode = (TsonArrayNodeBase)Activator.CreateInstance(targetNodeProperty.PropertyType);

                    newNode.Token = node.Token;
                    targetNodeProperty.SetPropertyValue(newNode);
                }
            }

            var lastTargetNodeProperty = targetNodeProperty;

            int i = 0;

            foreach (var subNode in node)
            {
                if (!typeof(TsonArrayNodeBase).IsAssignableFrom(lastTargetNodeProperty.PropertyType))
                    continue;

                targetNodeProperty = new NodePropertyInstance(lastTargetNodeProperty.Instance, lastTargetNodeProperty.PropertyInfo, i);

                Visit((TsonNode)subNode);

                i++;
            }

            targetNodeProperty = lastTargetNodeProperty;

            return node;
        }

        protected override TsonNode VisitNumber(TsonNumberNode node)
        {
            UpdateTargetNodeProperty(node);

            return node;
        }

        protected override TsonNode VisitString(TsonStringNode node)
        {
            UpdateTargetNodeProperty(node);

            return node;
        }

        protected override TsonNode VisitBoolean(TsonBooleanNode node)
        {
            UpdateTargetNodeProperty(node);

            return node;
        }
    }
}

