using System;

namespace JsonApi.ObjectModel
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ResourceIdAttribute : Attribute
    {
    }
}
