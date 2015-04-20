using System;
using System.IO;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;

namespace JsonApi
{
    public class JsonApiMediaTypeFormatter : JsonMediaTypeFormatter
    {
        public JsonApiMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/vnd.api+json"));
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return base.CanWriteType(type);
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
        {
            JsonWriter writer = CreateJsonWriter(type, writeStream, effectiveEncoding);
            writer.WriteComment("Hello JsonApi");
            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteValue(type.Name);
            writer.WriteEndObject();
            writer.Flush();
        }
    }
}
