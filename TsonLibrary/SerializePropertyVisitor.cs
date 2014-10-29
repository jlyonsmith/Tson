using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;

namespace TsonLibrary
{
    public class SerializePropertyVisitor : PropertyVisitor
    {
        TsonNode owningNode;

        private object SourceObject { get; set; }

        public SerializePropertyVisitor(object sourceObject)
        {
            this.SourceObject = sourceObject;
        }

        public TsonRootObjectNode ToTsonRootObjectNode()
        {
            var propInfo = this.GetType().GetProperty("SourceObject", BindingFlags.NonPublic | BindingFlags.Instance);

            this.owningNode = new TsonRootObjectNode();

            Visit(new InstanceProperty(this, propInfo, isRoot: true));

            return owningNode as TsonRootObjectNode;
        }

        private void AddToOwningNode(string name, TsonNode node)
        {
            if (this.owningNode.IsObject)
                ((TsonObjectNode)this.owningNode).KeyValues.Add(name, node);
            else
                ((TsonArrayNode)this.owningNode).Values.Add(node);
        }

        protected override InstanceProperty VisitObject(InstanceProperty ip)
        {
            TsonNode savedContainerNode = null;

            if (!ip.IsRoot)
            {
                var objNode = new TsonObjectNode();

                // Add entry in parent for this object node
                AddToOwningNode(ip.Name, objNode);
                savedContainerNode = this.owningNode;
                this.owningNode = objNode;
            }

            var instance = ip.GetIndexedValue();

            if (instance != null)
            {
                // Go through the property object looking for readable public properties
                var propInfos = instance.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (var propInfo in propInfos)
                {
                    if (propInfo.CanRead)
                    {
                        Visit(new InstanceProperty(instance, propInfo));
                    }
                }
            }

            if (savedContainerNode != null)
            {
                this.owningNode = savedContainerNode;
            }

            return ip;
        }

        protected override InstanceProperty VisitList(InstanceProperty ip)
        {
            var arrNode = new TsonArrayNode();

            // Add entry in parent for this object node
            AddToOwningNode(ip.Name, arrNode);

            var savedContainerNode = this.owningNode;

            this.owningNode = arrNode;

            IEnumerable enumerable = (IEnumerable)ip.GetIndexedValue();
            int i = 0;

            foreach (var item in enumerable)
            {
                Visit(new InstanceProperty(ip.Instance, ip.PropertyInfo, index: i));
                i++;
            }

            this.owningNode = savedContainerNode;

            return ip;
        }

        protected override InstanceProperty VisitNumberNode(InstanceProperty ip)
        {
            AddToOwningNode(ip.Name, ip.GetIndexedValue() as TsonNumberNode);

            return ip;
        }

        protected override InstanceProperty VisitStringNode(InstanceProperty ip)
        {
            AddToOwningNode(ip.Name, ip.GetIndexedValue() as TsonStringNode);

            return ip;
        }

        protected override InstanceProperty VisitBooleanNode(InstanceProperty ip)
        {
            AddToOwningNode(ip.Name, ip.GetIndexedValue() as TsonBooleanNode);

            return ip;
        }

        protected override InstanceProperty VisitObjectNode(InstanceProperty ip)
        {
            AddToOwningNode(ip.Name, ip.GetIndexedValue() as TsonObjectNode);

            return ip;
        }

        protected override InstanceProperty VisitArrayNode(InstanceProperty ip)
        {
            AddToOwningNode(ip.Name, ip.GetIndexedValue() as TsonArrayNode);

            return ip;
        }
    }
}

