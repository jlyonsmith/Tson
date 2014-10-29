using System;
using System.Reflection;
using System.Collections.Generic;

namespace TsonLibrary
{
    public class DeserializeTsonNodeVisitor<T> : TsonNodeVisitor where T:class
    {
        TsonRootObjectNode rootNode;
        InstanceProperty owner;

        protected T TargetObject { get; set; }

        public DeserializeTsonNodeVisitor(TsonRootObjectNode rootNode)
        {
            this.rootNode = rootNode;
        }

        public T ToInstance()
        {
            var propInfo = this.GetType().GetProperty("TargetObject", BindingFlags.NonPublic | BindingFlags.Instance);

            this.owner = new InstanceProperty(this, propInfo, isRoot: true);

            Visit(rootNode);

            return TargetObject;
        }

        private void UpdateOwnerObject(TsonNode node)
        {
            if (owner.IsListProperty)
            {
                var methodInfo = owner.PropertyType.GetMethod("Add", new Type[] { node.GetType() });

                if (methodInfo != null)
                    methodInfo.Invoke(owner.GetValue(), new object[] { node });
            }
            else
            {
                owner.SetValue(node);
            }
        }

        protected override TsonNode VisitRootObject(TsonRootObjectNode node)
        {
            return VisitObject((TsonObjectNode)node);
        }

        protected override TsonNode VisitObject(TsonObjectNode node)
        {
            var obj = Activator.CreateInstance(owner.IndexedType);

            owner.SetIndexedValue(obj);

            if (owner.IndexedType == typeof(TsonObjectNode))
            {
                // Just copy the whole node to the owner and don't visit the children
                owner.SetValue(node);
                return node;
            }

            var savedOwner = owner;

            foreach (var subNode in node.KeyValues)
            {
                var propInfo = savedOwner.IndexedType.GetProperty(subNode.Key.Value, BindingFlags.Public | BindingFlags.Instance);

                // If this key does not exist in the owner object, ignore it
                if (propInfo == null)
                    continue;

                owner = new InstanceProperty(obj, propInfo);

                Visit(subNode.Value);
            }

            owner = savedOwner;

            return node;
        }

        protected override TsonNode VisitArray(TsonArrayNode node)
        {
            var obj = Activator.CreateInstance(owner.IndexedType);

            owner.SetIndexedValue(obj);

            if (owner.IndexedType == typeof(TsonArrayNode))
            {
                // Just copy the whole node to the owner and don't visit the children
                owner.SetValue(node);
                return node;
            }

            var savedOwner = owner;

            int i = 0;

            foreach (var subNode in node.Values)
            {
                owner = new InstanceProperty(savedOwner.Instance, savedOwner.PropertyInfo, index: i);

                Visit(subNode);

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

