using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using JsonApi.Serialization;
using Newtonsoft.Json;

namespace JsonApi.ObjectModel
{
    [JsonConverter(typeof(JsonWriterJsonConverter))]
    public class LinkObject : IJsonWriter, IMeta
    {
        private readonly dynamic _innerExpando;
        private readonly IDictionary<string, object> _innerExpandoDict; 

        public List<ResourceObject> Resources { get; private set; }
        public LinkType LinkType { get; private set; }
        public bool Sideload { get; private set; }

        public List<ResourceIdentifier> Linkage { get; private set; }

        private LinkObject(List<ResourceObject> resources, LinkType linkType, bool sideload)
        {
            _innerExpando = new ExpandoObject();
            _innerExpandoDict = _innerExpando;

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

        private LinkObject(Uri uri)
        {
            _innerExpando = uri;
            _innerExpandoDict = new ExpandoObject();
            LinkType = LinkType.ToUrl;
            Resources = new List<ResourceObject>();
            Linkage = new List<ResourceIdentifier>();
        }

        public Uri Self
        {
            get { return ((ExpandoObject)_innerExpando).GetValueIfPresent<Uri>("Self"); }
            set { _innerExpando.Self = value; }
        }

        public Uri Related
        {
            get { return ((ExpandoObject)_innerExpando).GetValueIfPresent<Uri>("Related"); }
            set { _innerExpando.Related = value; }
        }

        public dynamic Meta
        {
            get
            {
                object ret;
                if (!_innerExpandoDict.TryGetValue("Meta", out ret))
                {
                    ret = new ExpandoObject();
                    _innerExpandoDict.Add("Meta", ret);
                }
                return ret;
            }
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

        public static LinkObject LinkToUri(Uri uri)
        {
            return new LinkObject(uri);
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            serializer.Serialize(writer, _innerExpando);
        }
    }
}
