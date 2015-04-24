using System;
using System.Collections.Generic;
using System.Dynamic;
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
        private readonly object _forObject;
        private readonly dynamic _innerExpando;
        private readonly IJsonApiProfile _profile;

        public ResourceObject(object forObject, IJsonApiProfile profile)
        {
            _forObject = forObject;
            _profile = profile;

            if (forObject != null)
            {
                ValidateResourceObjectAttribute(forObject);
                ValidateResourceFieldNames(forObject);
                _innerExpando = DynamicExtensions.InitializeExpandoFromPublicObjectProperties(forObject);
                _innerExpando.Id = GetResourceId(forObject);
                _innerExpando.Type = GetResourceType(forObject, profile.Inflector);

                Resourcify(forObject, _innerExpando, profile);
            }
        }

        public ResourceIdentifier ResourceIdentifier
        {
            get
            {
                if (_forObject != null)
                {
                    return new ResourceIdentifier(_innerExpando.Type, _innerExpando.Id);
                }
                return null;
            }
        }

        public IEnumerable<LinkObject> Links
        {
            get
            {
                var expandoDict = (IDictionary<string, object>)_innerExpando;
                if (expandoDict.ContainsKey("Links"))
                {
                    return expandoDict["Links"] as IEnumerable<LinkObject>;
                }
                return new List<LinkObject>();
            }
        }

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
                foreach (var disalowedPropertyName in new[] { "Type", "Links", "Meta", "Self" })
                {
                    if (memberInfo.Name.Equals(disalowedPropertyName, StringComparison.CurrentCultureIgnoreCase))
                    {
                        throw new JsonApiSpecException("Resource object class {0} cannot have a '{1}' property or attribute", 
                            memberInfo.ReflectedType.Name, disalowedPropertyName);
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
        /// Convert child objects marked with the [Resource] attribute into ResourceObject instances
        /// </summary>
        public static void Resourcify(object forObject, IDictionary<string, object> expandoDict, IJsonApiProfile withProfile)
        {
            foreach (var propertyInfo in forObject.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (propertyInfo.PropertyType.IsDefined(typeof(ResourceObjectAttribute)))
                {
                    expandoDict[propertyInfo.Name] = new ResourceObject(propertyInfo.GetValue(forObject), withProfile);
                }
                xxx // Add IEnumerable support to Resourcify()
            }
            foreach (var fieldInfo in forObject.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                if (fieldInfo.FieldType.IsDefined(typeof(ResourceObjectAttribute)))
                {
                    expandoDict[fieldInfo.Name] = new ResourceObject(fieldInfo.GetValue(forObject), withProfile);
                }
            }
        }

        public IEnumerable<ResourceObject> ExtractAndRewireResourceLinks()
        {
            if (_forObject == null)
            {
                return new HashSet<ResourceObject>();
            }

            var expandoDict = (IDictionary<string, object>)_innerExpando;
            var accumulatedResources = new HashSet<ResourceObject>();

            // Iterate over the expando properties looking for ResourceObjects.
            // If found, remove the object and add its link to the return value
            // and add or expand an existing link relationship.
            foreach (KeyValuePair<string, object> kvp in expandoDict.ToList())
            {
                if (kvp.Value is ResourceObject)
                {
                    var ro = kvp.Value as ResourceObject;
                    if (ro != null)
                    {
                        accumulatedResources.Add(ro);
                        foreach (var subRo in ro.ExtractAndRewireResourceLinks())
                        {
                            accumulatedResources.Add(subRo);
                        }
                    }
                    AddOrExpandLink(kvp.Key, ro.ResourceIdentifier, LinkType.ToOne);
                    expandoDict.Remove(kvp);
                }
                if (kvp.Value is IEnumerable<ResourceObject>)
                {
                    var ros = kvp.Value as IEnumerable<ResourceObject>;
                    foreach (var ro in ros)
                    {
                        if (ro != null)
                        {
                            accumulatedResources.Add(ro);
                            foreach (var subRo in ro.ExtractAndRewireResourceLinks())
                            {
                                accumulatedResources.Add(subRo);
                            }
                        }
                        AddOrExpandLink(kvp.Key, ro.ResourceIdentifier, LinkType.ToMany);
                    }
                    expandoDict.Remove(kvp);
                }
            }

            return accumulatedResources;
        }

        private void AddOrExpandLink(string relationship, ResourceIdentifier identifier, LinkType linkType)
        {
            // Initialize Links section if not already
            var expandoDict = (IDictionary<string, object>)_innerExpando;
            if (!expandoDict.ContainsKey("Links"))
            {
                expandoDict.Add("Links", new Dictionary<string, object>());
            }

            // Initialize link if not already
            var linksDict = (IDictionary<string, object>)expandoDict["Links"];
            if (!linksDict.ContainsKey(relationship))
            {
                switch (linkType)
                {
                    case LinkType.ToOne:
                        linksDict.Add(relationship, identifier);
                        break;
                    case LinkType.ToMany:
                        linksDict.Add(relationship, new List<ResourceIdentifier> {identifier});
                        break;
                }
            }
            else
            {
                switch (linkType)
                {
                    case LinkType.ToOne:
                        throw new JsonApiSpecException("Cannot assign multiple values to a ToOne relationship");
                    case LinkType.ToMany:
                        ((List<ResourceIdentifier>)linksDict[relationship]).Add(identifier);
                        break;
                }
            }
        }

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
                if (contract is JsonObjectContract)
                {
                    var objectContract = contract as JsonObjectContract;
                    if (type == typeof(ResourceObject) || type == typeof(ResourceIdentifier))
                    {
                        // Verify that a ResourceObject has the required fields
                        foreach (var requiredPropertyName in new[] { "id", "type" })
                        {
                            JsonProperty reqProp = objectContract.Properties.FirstOrDefault(p => p.PropertyName.Equals(requiredPropertyName, StringComparison.InvariantCultureIgnoreCase));
                            if (reqProp == null)
                            {
                                throw new JsonApiSpecException("Resource object missing required '{0}' json property", requiredPropertyName);
                            }
                        }
                    }
                    else
                    {
                        // Verify that the json attributes of complex attributes don't violate reserved key rules.
                        foreach (var property in objectContract.Properties)
                        {
                            foreach (var disalowedPropertyName in new[] { "id", "type", "links", "meta" })
                            {
                                if (property.PropertyName.Equals(disalowedPropertyName, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    throw new JsonApiSpecException("Complex attribute of type {0} cannot have a '{1}' json property", objectContract.UnderlyingType.Name, disalowedPropertyName);
                                }
                            }
                        }
                    }  
                }
                return contract;
            }
        }
    }
}
