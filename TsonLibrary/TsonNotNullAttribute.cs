using System;

namespace TsonLibrary
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class TsonNotNullAttribute : Attribute
    {
        public TsonNotNullAttribute()
        {
        }
    }
}

