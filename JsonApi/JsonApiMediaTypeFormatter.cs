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
//            SerializerSettings.ContractResolver = new JsonApiContractResolver();
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
            try
            {
                // Buffer the output in case there is an error
                var nominalStream = new MemoryStream();

                IJsonWriter resourceDocument;
                if (value is IEnumerable<object>)
                {
                    resourceDocument = new ResourceDocument((value as IEnumerable<object>).Select(o => new ResourceObject(o)));
                }
                else
                {
                    resourceDocument = new ResourceDocument(new ResourceObject(value));
                }

                JsonWriter writer = CreateJsonWriter(type, nominalStream, effectiveEncoding);
                JsonSerializer serializer = CreateJsonSerializer();
                writer.Formatting = Formatting.Indented;

                serializer.Serialize(writer, resourceDocument);
                writer.Flush();
                nominalStream.Position = 0;
                nominalStream.CopyTo(writeStream);
                writeStream.Flush();
            }
            catch (JsonApiSpecException ex)
            {
                var errorStream = new MemoryStream();
                IJsonWriter errorDocument = new ResourceDocument(new []
                {
                    new Error
                    {
                        Code = "JsonApiSpecViolation",
                        Title = "JsonApi Specification Violation",
                        Href = "http://jsonapi.org/format",
                        Status = "500",
                        Detail = ex.Message
                    }, 
                });
                JsonWriter writer = CreateJsonWriter(type, errorStream, effectiveEncoding);
                JsonSerializer serializer = CreateJsonSerializer();
                writer.Formatting = Formatting.Indented;

                serializer.Serialize(writer, errorDocument);
                writer.Flush();
                errorStream.Position = 0;
                errorStream.CopyTo(writeStream);
                writeStream.Flush();
            }
        }
    }
}
