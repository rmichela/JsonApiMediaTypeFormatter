using System;

namespace JsonApi.ObjectModel
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ResourceRelationshipAttribute : Attribute
    {
        public bool Sideload { get; set; }
    }
}
