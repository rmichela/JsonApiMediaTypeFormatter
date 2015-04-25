using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using JsonApi.Serialization;
using Newtonsoft.Json;

namespace JsonApi.ObjectModel
{
    [JsonConverter(typeof(JsonWriterJsonConverter))]
    internal class LinkObject : IJsonWriter
    {
        private readonly dynamic _innerExpando = new ExpandoObject();

        public List<ResourceObject> Resources { get; private set; }
        public LinkType LinkType { get; private set; }
        public bool Sideload { get; private set; }

        public List<ResourceIdentifier> Linkage { get; private set; }

        private LinkObject(List<ResourceObject> resources, LinkType linkType, bool sideload)
        {
            LinkType = linkType;
            Sideload = sideload;
            Resources = sideload ? resources : new List<ResourceObject>();
            Linkage = resources.Where(r => r != null).Select(r => r.ResourceIdentifier).ToList();

            if (linkType == LinkType.ToOne)
            {
                _innerExpando.Linkage = Linkage.FirstOrDefault();
            }
            if (linkType == LinkType.ToMany)
            {
                _innerExpando.Linkage = Linkage;
            }
        }

        public Uri Self
        {
            get { return _innerExpando.Self; }
            set { _innerExpando.Self = value; }
        }

        public Uri Related
        {
            get { return _innerExpando.Related; }
            set { _innerExpando.Related = value; }
        }

        public object Meta
        {
            get { return _innerExpando.Meta; }
            set { _innerExpando.Meta = value; }
        }

        public static LinkObject Empty(LinkType linkType)
        {
            return new LinkObject(new List<ResourceObject>(), linkType, false);
        }

        public static LinkObject LinkToMany(List<ResourceObject> resources, bool sideload)
        {
            return new LinkObject(resources, LinkType.ToMany, sideload);
        }

        public static LinkObject LinkToOne(ResourceObject resource, bool sideload)
        {
            return new LinkObject(new List<ResourceObject>{resource}, LinkType.ToOne, sideload );
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            serializer.Serialize(writer, _innerExpando);
        }
    }
}
