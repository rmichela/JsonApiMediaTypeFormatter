using System;

namespace JsonApi.ObjectModel
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ResourceObjectAttribute : Attribute
    {
        public string Type { get; set; }
    }
}
