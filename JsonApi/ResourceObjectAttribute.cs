using System;

namespace JsonApi
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ResourceObjectAttribute : Attribute
    {
        public string Type { get; set; }
    }
}
