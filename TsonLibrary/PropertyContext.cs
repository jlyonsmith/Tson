using System;
using System.Reflection;
using System.Collections.Generic;

namespace TsonLibrary
{
    public class PropertyContext 
    {
        public PropertyContext(object instance, PropertyInfo propInfo, int index = -1)
        {
            this.Instance = instance;
            this.PropertyInfo = propInfo;
            this.Index = index;

            if (HasIndex && !typeof(TsonArrayNodeBase).IsAssignableFrom(PropertyType))
                throw new ArgumentException("Index cannot be specified for non-array nodes");
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
        public void SetPropertyValue(object obj)
        {
            this.PropertyInfo.SetValue(Instance, obj);
        }
        public void SetItemValue(object obj)
        {
            // NOTE: This assumes an indexer that can accept an arbitrary index and still work 
            // Not List<> for example.
            var itemPropInfo = PropertyType.GetProperty("Item");

            if (itemPropInfo == null)
                return;

            itemPropInfo.SetValue(this.GetPropertyValue(), obj, new object[] { (object)this.Index });
        }
    }
}

