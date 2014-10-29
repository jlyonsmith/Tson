using System;
using System.Reflection;
using System.Collections.Generic;

namespace TsonLibrary
{
    public class InstanceProperty 
    {
        bool isListProperty;

        public InstanceProperty(object instance, PropertyInfo propInfo, int index = -1, bool isRoot = false)
        {
            this.Instance = instance;
            this.PropertyInfo = propInfo;
            this.IsRoot = isRoot;
            this.Index = index;

            PropertyType = propInfo.PropertyType;
            isListProperty = PropertyType.IsGenericType && PropertyType.GetGenericTypeDefinition() == typeof(List<>); 

            if (isListProperty && index >= 0)
                IndexedType = PropertyType.GetGenericArguments()[0];
            else
                IndexedType = PropertyType;
        }

        public bool IsRoot { get; private set; }
        public int Index { get; private set; }
        public object Instance { get; private set; }
        public Type IndexedType { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        public Type PropertyType { get; private set; }
        public bool IsListProperty { get { return isListProperty; } }
        public string Name { get { return this.PropertyInfo.Name; } }
        public object GetValue()
        {
            return this.PropertyInfo.GetValue(this.Instance);
        }
        public object GetIndexedValue()
        {
            var obj = GetValue();

            if (this.Index >= 0)
            {
                var propInfo = PropertyType.GetProperty("Item");

                return propInfo.GetValue(obj, new object[] { (object)this.Index });
            }
            else
                return obj;
        }
        public void SetValue(object obj)
        {
            this.PropertyInfo.SetValue(Instance, obj);
        }
        public void SetIndexedValue(object obj)
        {
            if (this.Index >= 0)
            {
                var itemPropInfo = PropertyType.GetProperty("Item");
                var countPropInfo = PropertyType.GetProperty("Count");
                var addMethodInfo = PropertyType.GetMethod("Add");

                if (itemPropInfo == null || countPropInfo == null || addMethodInfo == null)
                    return;

                // Ensure the given index is valid
                int count = -1;
                object coll = this.GetValue();

                while (count < this.Index)
                {
                    count = (int)countPropInfo.GetValue(coll);
                    addMethodInfo.Invoke(coll, new object[] { null });
                }

                itemPropInfo.SetValue(this.GetValue(), obj, new object[] { (object)this.Index });
            }
            else
            {
                SetValue(obj);
            }
        }
    }
}

