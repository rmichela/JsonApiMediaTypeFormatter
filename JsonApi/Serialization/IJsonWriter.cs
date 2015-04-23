using Newtonsoft.Json;

namespace JsonApi.Serialization
{
    internal interface IJsonWriter
    {
        void WriteJson(JsonWriter writer, JsonSerializer serializer);
    }
}
