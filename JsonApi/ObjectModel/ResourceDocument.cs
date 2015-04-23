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

        public ResourceDocument(IEnumerable<ResourceObject> data)
        {
            if (data.Any())
            {
                string firstType = data.First().Type;
                if (data.Any(d => d.Type != firstType))
                {
                    throw new JsonApiSpecException("All top-level resources in a document must be of the same type");
                }
            }
            _innerExpando.Data = data;
        }

        public ResourceDocument(IEnumerable<Error> errors)
        {
            _innerExpando.Errors = errors;
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            serializer.Serialize(writer, _innerExpando);
        }
    }
}
