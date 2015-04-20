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
            if (type == typeof (string))
            {
                return false;
            }
            return base.CanWriteType(type);
        }

        public override void WriteToStream(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
        {
            var rootResource = new ResourceObject(value);

            JsonWriter writer = CreateJsonWriter(type, writeStream, effectiveEncoding);
            writer.Formatting = Formatting.Indented; 

            rootResource.WriteJson(writer);
            
            writer.Flush();
        }
    }
}
