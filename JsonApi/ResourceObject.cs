using System.Collections.Generic;
using System.Data.Entity.Design.PluralizationServices;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace JsonApi
{
    internal class ResourceObject
    {
        private readonly dynamic _innerExpando;

        public ResourceObject(object forObject)
        {
            ValidateResourceObjectAttribute(forObject);
            _innerExpando = DynamicExtensions.InitializeExpandoFromPublicObjectProperties(forObject);
            ValidateFieldNames(_innerExpando);
            _innerExpando.Id = GetResourceId(forObject);
            _innerExpando.Type = GetResourceType(forObject);
        }

        public static void ValidateResourceObjectAttribute(object forObject)
        {
            if (!forObject.GetType().IsDefined(typeof(ResourceObjectAttribute)))
            {
                throw new JsonApiException("Top level objects must be resource objects as denoted by the [ResourceObject] attribute");
            }
        }

        public static void ValidateFieldNames(IDictionary<string, object> expando)
        {
            if (expando.ContainsKey("type"))
            {
                throw new JsonApiException("Resource object classes cannot have a 'type' property");
            }
            if (expando.ContainsKey("links"))
            {
                throw new JsonApiException("Resource object classes cannot have a 'links' property");
            }
            if (expando.ContainsKey("meta"))
            {
                throw new JsonApiException("Resource object classes cannot have a 'meta' property");
            }
        }

        public static string GetResourceId(object forObject)
        {
            object idValue = null;

            var type = forObject.GetType();
            var idPropertiesByAttribute = type.GetProperties().Where(p => p.IsDefined(typeof(ResourceIdAttribute), true)).ToList();
            if (idPropertiesByAttribute.Count > 1)
            {
                throw new JsonApiException("Resource objects may only have one [ResourceId] attribute");
            }
            if (idPropertiesByAttribute.Count == 1)
            {
                idValue = idPropertiesByAttribute.First().GetValue(forObject);
            }
            if (idPropertiesByAttribute.Count == 0)
            {
                PropertyInfo idProperty = forObject.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
                if (idProperty != null)
                {
                    idValue = idProperty.GetValue(forObject);
                }
                else
                {
                    throw new JsonApiException("Resource objects must have an Id field");
                }
            }

            if (idValue == null || string.IsNullOrWhiteSpace(idValue.ToString()))
            {
                throw new JsonApiException("Resource object Ids cannot be null or empty");
            }

            return idValue.ToString();
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
