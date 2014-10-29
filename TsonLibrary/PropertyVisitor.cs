using System;
using System.Collections.Generic;
using System.Reflection;

namespace TsonLibrary
{
    public abstract class PropertyVisitor
    {
        protected virtual InstanceProperty Visit(InstanceProperty ip)
        {
            if (ip == null)
                return null;

            if (ip.IndexedType == typeof(TsonNumberNode))
                return VisitNumberNode(ip);
            else if (ip.IndexedType == typeof(TsonStringNode))
                return VisitStringNode(ip);
            else if (ip.IndexedType == typeof(TsonArrayNode))
                return VisitArrayNode(ip);
            else if (ip.IndexedType == typeof(TsonBooleanNode))
                return VisitBooleanNode(ip);
            else if (ip.IndexedType == typeof(TsonNullNode))
                return VisitNullNode(ip);
            else if (ip.IndexedType == typeof(TsonObjectNode))
                return VisitObjectNode(ip);
            else if (ip.IsListProperty && ip.Index < 0)
                return VisitList(ip);
            else
                return VisitObject(ip);
        }

        protected virtual InstanceProperty VisitObject(InstanceProperty ip)
        {
            return ip;
        }

        protected virtual InstanceProperty VisitNumberNode(InstanceProperty ip)
        {
            return ip;
        }

        protected virtual InstanceProperty VisitStringNode(InstanceProperty ip)
        {
            return ip;
        }

        protected virtual InstanceProperty VisitBooleanNode(InstanceProperty ip)
        {
            return ip;
        }

        protected virtual InstanceProperty VisitNullNode(InstanceProperty ip)
        {
            return ip;
        }

        protected virtual InstanceProperty VisitArrayNode(InstanceProperty ip)
        {
            return ip;
        }

        protected virtual InstanceProperty VisitObjectNode(InstanceProperty ip)
        {
            return ip;
        }

        protected virtual InstanceProperty VisitList(InstanceProperty ip)
        {
            return ip;
        }
    }
}

