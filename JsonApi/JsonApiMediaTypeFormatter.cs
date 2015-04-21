using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            IJsonWriter rootResource;
            if (value is IEnumerable<object>)
            {
                rootResource = new ResourceDocument((value as IEnumerable<object>).Select(o => new ResourceObject(o)));
            }
            else
            {
                rootResource = new ResourceDocument(new ResourceObject(value));
            }

            JsonWriter writer = CreateJsonWriter(type, writeStream, effectiveEncoding);
            JsonSerializer serializer = CreateJsonSerializer();
            writer.Formatting = Formatting.Indented;

            serializer.Serialize(writer, rootResource);

            writer.Flush();
        }
    }
}
