using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using JsonApi.Profile;
using JsonApi.Serialization;
using Newtonsoft.Json;

namespace JsonApi.ObjectModel
{
    [JsonConverter(typeof(JsonWriterJsonConverter))]
    internal class ResourceDocument : IJsonWriter
    {
        private readonly IJsonApiProfile _profile;
        private readonly dynamic _innerExpando = new ExpandoObject();

        public ResourceDocument(ResourceObject data, IJsonApiProfile profile)
        {
            _profile = profile;
            _innerExpando.Data = data;
            ExtractIncludedLinks(data);
        }

        public ResourceDocument(List<ResourceObject> data, IJsonApiProfile profile)
        {
            _profile = profile;

            ValidateResourceObjectCollectionSameType(data);
            ValidateResourceObjectCollectionUniqueness(data);
            data.ForEach(ExtractIncludedLinks);
            
            _innerExpando.Data = data;
        }

        public ResourceDocument(IEnumerable<Error> errors)
        {
            _innerExpando.Errors = errors;
        }

        private void ExtractIncludedLinks(ResourceObject resource)
        {
            var expandoDict = (IDictionary<string, object>)_innerExpando;
            if (!expandoDict.ContainsKey("Included"))
            {
                expandoDict.Add("Included", new HashSet<ResourceObject>());
            }
            foreach (var ro in resource.ExtractAndRewireResourceLinks())
            {
                ((HashSet<ResourceObject>)expandoDict["Included"]).Add(ro);
            }
        }

        public static void ValidateResourceObjectCollectionSameType(IEnumerable<ResourceObject> resources)
        {
            if (resources.Any())
            {
                string firstType = resources.First().ResourceIdentifier.Type;
                if (resources.Any(d => d.ResourceIdentifier.Type != firstType))
                {
                    throw new JsonApiSpecException("All top-level resources in a document must be of the same type");
                }
            }
        }

        public static void ValidateResourceObjectCollectionUniqueness(IEnumerable<ResourceObject> resources)
        {
            var seenKeys = new HashSet<ResourceIdentifier>();
            foreach (var resourceObject in resources)
            {
                if (!seenKeys.Add(resourceObject.ResourceIdentifier))
                {
                    throw new JsonApiSpecException("Resource object {0}:{1} included more than once",
                        resourceObject.ResourceIdentifier.Type, resourceObject.ResourceIdentifier.Id);
                }
            }
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            serializer.Serialize(writer, _innerExpando);
        }
    }
}
