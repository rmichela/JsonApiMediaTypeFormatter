using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using JsonApi.Profile;
using JsonApi.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JsonApi.ObjectModel
{
    [JsonConverter(typeof(JsonWriterJsonConverter))]
    internal class ResourceObject : IJsonWriter
    {
        private readonly dynamic _innerExpando;
        private readonly IJsonApiProfile _profile;

        public ResourceObject(object forObject, IJsonApiProfile profile)
        {
            _profile = profile;

            ValidateResourceObjectAttribute(forObject);
            ValidateResourceFieldNames(forObject);
            _innerExpando = DynamicExtensions.InitializeExpandoFromPublicObjectProperties(forObject);            
            _innerExpando.Id = GetResourceId(forObject);
            _innerExpando.Type = GetResourceType(forObject, profile.Inflector);
        }

        public string Id { get { return _innerExpando.Id; } }
        public string Type { get { return _innerExpando.Type; } }

        public static void ValidateResourceObjectAttribute(object forObject)
        {
            if (!forObject.GetType().IsDefined(typeof(ResourceObjectAttribute)))
            {
                throw new JsonApiSpecException("Top level objects must be resource objects as denoted by the [ResourceObject] attribute");
            }
        }

        public static void ValidateResourceFieldNames(object forObject)
        {
            foreach (var memberInfo in forObject.GetType().GetMembers(BindingFlags.Instance | BindingFlags.Public))
            {
                foreach (var disalowedPropertyName in new[] { "Type", "Links", "Meta" })
                {
                    if (memberInfo.Name.Equals(disalowedPropertyName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        throw new JsonApiSpecException(string.Format("Resource object class {0} cannot have a '{1}' property or attribute", 
                            memberInfo.ReflectedType.Name, disalowedPropertyName));
                    }
                }
            }
        }

        public static string GetResourceId(object forObject)
        {
            object idValue = null;

            var type = forObject.GetType();
            var idPropertiesByAttribute = type.GetProperties().Where(p => p.IsDefined(typeof(ResourceIdAttribute), true)).ToList();
            if (idPropertiesByAttribute.Count > 1)
            {
                throw new JsonApiSpecException("Resource objects may only have one [ResourceId] attribute");
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
                    throw new JsonApiSpecException("Resource objects must have an Id field or a field marked with the [ResourceId] attribute");
                }
            }

            if (idValue == null || string.IsNullOrWhiteSpace(idValue.ToString()))
            {
                throw new JsonApiSpecException("Resource object Ids cannot be null or empty");
            }

            return idValue.ToString();
        }

        public static string GetResourceType(object forObject, IInflector inflector)
        {
            var resourceAttribute = forObject.GetType().GetCustomAttribute<ResourceObjectAttribute>(false);
            string typeName = forObject.GetType().Name;
            return resourceAttribute.Type ?? inflector.Pluralize(typeName);
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            IContractResolver existingResolver = serializer.ContractResolver;
            serializer.ContractResolver = new ComplexAttributeFieldNameEnforcingContractResolver(existingResolver);
            serializer.Serialize(writer, _innerExpando);
            serializer.ContractResolver = existingResolver;
        }

        /// <summary>
        /// Verifies that the json attributes of complex attributes don't violate reserved key rules.
        /// </summary>
        public class ComplexAttributeFieldNameEnforcingContractResolver : IContractResolver
        {
            private readonly IContractResolver _innerContractResolver;

            public ComplexAttributeFieldNameEnforcingContractResolver(IContractResolver innerContractResolver)
            {
                _innerContractResolver = innerContractResolver;
            }

            public JsonContract ResolveContract(Type type)
            {
                JsonContract contract = _innerContractResolver.ResolveContract(type);
                if (contract is JsonObjectContract && type != typeof(ResourceObject))
                {
                    var objectContract = contract as JsonObjectContract;
                    foreach (var property in objectContract.Properties)
                    {
                        foreach (var disalowedPropertyName in new[] {"id", "type", "links", "meta"})
                        {
                            if (property.PropertyName.Equals(disalowedPropertyName, StringComparison.CurrentCultureIgnoreCase))
                            {
                                throw new JsonApiSpecException(string.Format("Complex attribute of type {0} cannot have a '{1}' json property", objectContract.UnderlyingType.Name, disalowedPropertyName));
                            }
                        }
                    }
                }

                return contract;
            }
        }
    }
}
