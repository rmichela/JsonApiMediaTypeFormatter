using System;
using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonApi
{
    internal class ResourceObject
    {
        private readonly dynamic _innerExpando;

        public ResourceObject(object forObject)
        {
            _innerExpando = DynamicExtensions.InitializeExpandoFromPublicObjectProperties(forObject);
            ValidateFieldNames(_innerExpando);
            _innerExpando.Id = GetResourceId(forObject);
            _innerExpando.Type = GetResourceType(forObject);
        }

        public static void ValidateFieldNames(IDictionary<string, object> expando)
        {
            if (expando.ContainsKey("type"))
            {
                throw new JsonApiException("Resource objects cannot have 'type' properties");
            }
            if (expando.ContainsKey("links"))
            {
                throw new JsonApiException("Resource objects cannot have 'links' properties");
            }
            if (expando.ContainsKey("meta"))
            {
                throw new JsonApiException("Resource objects cannot have 'meta' properties");
            }
        }

        public static string GetResourceId(object forObject)
        {
            PropertyInfo idProperty = forObject.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
            return idProperty.GetValue(forObject).ToString();
        }

        public static string GetResourceType(object forObject)
        {
            string typeName = forObject.GetType().Name;
            var inflector = PluralizationService.CreateService(CultureInfo.CurrentCulture);
            return inflector.Pluralize(typeName);
        }

        public void WriteJson(JsonWriter writer)
        {
            var serializer = new JsonSerializer();
            serializer.Serialize(writer, _innerExpando);
        }
    }
}
