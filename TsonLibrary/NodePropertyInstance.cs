using System;
using System.Reflection;
using System.Collections.Generic;

namespace TsonLibrary
{
    internal class NodePropertyInstance 
    {
        public NodePropertyInstance(object instance, PropertyInfo propInfo, int index = -1)
        {
            this.Instance = instance;
            this.PropertyInfo = propInfo;
            this.Index = index;
        }

        public object Instance { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public int Index { get; private set; }

        public bool HasIndex { get { return Index >= 0; } }
        public Type ItemType { get { return PropertyType.IsGenericType ? PropertyType.GetGenericArguments()[0] : null; } }
        public Type PropertyType { get { return PropertyInfo.PropertyType; } }

        public string PropertyName { get { return this.PropertyInfo.Name; } }

        public object GetPropertyValue()
        {
            return this.PropertyInfo.GetValue(this.Instance);
        }
        public object GetItemValue()
        {
            var obj = GetPropertyValue();
            var propInfo = PropertyType.GetProperty("Item");

            return propInfo.GetValue(obj, new object[] { (object)this.Index });
        }
        public void SetPropertyValue(TsonNode node)
        {
            try
            {
                this.PropertyInfo.SetValue(Instance, node);
            }
            catch (Exception e)
            {
                throw new TsonFormatException(node.Token, 
                    String.Format("Cannot set non-array property {0}", PropertyInfo.Name), e);
            }
        }
        public void SetItemValue(TsonNode node)
        {
            // NOTE: This assumes an indexer that can accept an arbitrary index and still work 
            // Not List<> for example.
            var itemPropInfo = PropertyType.GetProperty("Item");

            if (itemPropInfo == null)
                return;

            try
            {
                itemPropInfo.SetValue(this.GetPropertyValue(), node, new object[] { (object)this.Index });
            }
            catch (Exception e)
            {
                throw new TsonFormatException(node.Token, 
                    String.Format("Cannot set array property", PropertyInfo.Name), e);
            }
        }
    }
}

