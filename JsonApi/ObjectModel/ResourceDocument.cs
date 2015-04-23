using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using JsonApi.Serialization;
using Newtonsoft.Json;

namespace JsonApi.ObjectModel
{
    [JsonConverter(typeof(JsonWriterJsonConverter))]
    internal class ResourceDocument : IJsonWriter
    {
        private readonly dynamic _innerExpando = new ExpandoObject();

        public ResourceDocument(ResourceObject data)
        {
            _innerExpando.Data = data;
        }

        public ResourceDocument(List<ResourceObject> data)
        {
            ValidateResourceObjectCollectionSameType(data);
            ValidateResourceObjectCollectionUniqueness(data);
            
            _innerExpando.Data = data;
        }

        public ResourceDocument(IEnumerable<Error> errors)
        {
            _innerExpando.Errors = errors;
        }

        public static void ValidateResourceObjectCollectionSameType(IEnumerable<ResourceObject> resources)
        {
            if (resources.Any())
            {
                string firstType = resources.First().Type;
                if (resources.Any(d => d.Type != firstType))
                {
                    throw new JsonApiSpecException("All top-level resources in a document must be of the same type");
                }
            }
        }

        public static void ValidateResourceObjectCollectionUniqueness(IEnumerable<ResourceObject> resources)
        {
            var seenKeys = new HashSet<Tuple<string, string>>();
            foreach (var resourceObject in resources)
            {
                var key = new Tuple<string, string>(resourceObject.Type, resourceObject.Id);
                if (!seenKeys.Add(key))
                {
                    throw new JsonApiSpecException("Resource object {0}:{1} included more than once", key.Item1, key.Item2);
                }
            }
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            serializer.Serialize(writer, _innerExpando);
        }
    }
}
