using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using Newtonsoft.Json;

namespace JsonApi
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
                    throw new JsonApiException("All top-level resources in a document must be of the same type");
                }
            }
            _innerExpando.Data = data;
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            serializer.Serialize(writer, _innerExpando);
        }
    }
}
