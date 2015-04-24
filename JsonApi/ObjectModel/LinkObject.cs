using System;
using System.Collections.Generic;

namespace JsonApi.ObjectModel
{
    internal class LinkObject
    {
        public Uri Self { get; set; }
        public Uri Related { get; set; }
        public List<ResourceIdentifier> Linkage { get; set; }
        public object Meta { get; set; }
    }
}
