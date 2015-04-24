using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using JsonApi.ObjectModel;
using JsonApi.Profile;
using Newtonsoft.Json;

namespace JsonApi.Serialization
{
    public class JsonApiMediaTypeFormatter : JsonMediaTypeFormatter
    {
        private readonly IJsonApiProfile _profile;

        public JsonApiMediaTypeFormatter() : this(new RecommendedProfile())
        {
            
        }

        public JsonApiMediaTypeFormatter(IJsonApiProfile profile)
        {
            _profile = profile;
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
                WriteDocument(type, value, writeStream, effectiveEncoding);
            }
            catch (JsonApiSpecException ex)
            {
                WriteSpecError(type, value, writeStream, effectiveEncoding, ex.Message);
            }
        }

        private void WriteDocument(Type type, object value, Stream writeStream, Encoding effectiveEncoding)
        {
            // Buffer the output in case there is an error
            var nominalStream = new MemoryStream();

            IJsonWriter resourceDocument;
            if (value is IEnumerable<object>)
            {
                var resourceObjectList = (value as IEnumerable<object>).Select(o => new ResourceObject(o, _profile)).ToList();
                resourceDocument = new ResourceDocument(resourceObjectList, _profile);
            }
            else
            {
                resourceDocument = new ResourceDocument(new ResourceObject(value, _profile), _profile);
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

        private void WriteSpecError(Type type, object value, Stream writeStream, Encoding effectiveEncoding, string detail)
        {
            var errorStream = new MemoryStream();
            IJsonWriter errorDocument = new ResourceDocument(new[]
                {
                    new Error
                    {
                        Code = "JsonApiSpecViolation",
                        Title = "JsonApi Specification Violation",
                        Href = "http://jsonapi.org/format",
                        Status = "500",
                        Detail = detail
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
