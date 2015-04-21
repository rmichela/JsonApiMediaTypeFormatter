using Newtonsoft.Json;

namespace JsonApi
{
    internal interface IJsonWriter
    {
        void WriteJson(JsonWriter writer, JsonSerializer serializer);
    }
}
