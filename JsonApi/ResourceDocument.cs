using System.Collections.Generic;
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
            _innerExpando.Included = data;
        }

        public ResourceDocument(IEnumerable<ResourceObject> data)
        {
            _innerExpando.Included = data;
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            serializer.Serialize(writer, _innerExpando);
        }
    }
}
