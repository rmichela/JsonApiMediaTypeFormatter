using System;
using Newtonsoft.Json;

namespace JsonApi.Serialization
{
    internal class JsonWriterJsonConverter : JsonConverter 
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jw = value as IJsonWriter;
            if (jw != null)
            {
                jw.WriteJson(writer, serializer);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (IJsonWriter).IsAssignableFrom(objectType);
        }
    }
}
