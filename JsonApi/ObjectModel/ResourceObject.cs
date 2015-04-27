using System;
using System.Collections;
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
                _innerExpando = TypeExtensions.InitializeExpandoFromPublicObjectProperties(forObject);
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
            foreach (var memberInfo in forObject.GetType().GetMembers(TypeExtensions.PublicInstance))
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
            var idPropertiesByAttribute = type.GetPropertiesAndFields(TypeExtensions.PublicInstance)
                .Where(p => p.IsDefined(typeof(ResourceIdAttribute), true))
                .ToList();
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
                PropertyFieldInfo idProperty = forObject.GetType().GetPropertyOrField("Id", TypeExtensions.PublicInstance);
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

        /// <summary>
        /// Convert child objects marked with the [Resource] attribute into ResourceObject instances
        /// </summary>
        public static void Resourcify(object forObject, IDictionary<string, object> expandoDict, IJsonApiProfile withProfile)
        {
            foreach (var propertyInfo in forObject.GetType().GetPropertiesAndFields(TypeExtensions.PublicInstance))
            {
                var resRel = propertyInfo.GetCustomAttribute<ResourceRelationshipAttribute>();
                if (resRel != null)
                {
                    Type enumerableType = propertyInfo.OfType.GetGenericIEnumerables().FirstOrDefault();
                    var propValue = propertyInfo.GetValue(forObject);
                    if (enumerableType != null)
                    {
                        if (propValue != null)
                        {
                            // It's a to-many relationship
                            var resources = (((IEnumerable)propValue))
                                .Cast<object>()
                                .Select(o => new ResourceObject(o, withProfile))
                                .ToList();
                            expandoDict[propertyInfo.Name] = LinkObject.LinkToMany(resources, resRel.Sideload);
                        }
                        else
                        {
                            expandoDict[propertyInfo.Name] = LinkObject.Empty(LinkType.ToMany);
                        }
                    }
                    else
                    {
                        // It's a to-one relationship
                        if (propValue != null)
                        {
                            var resourceObject = new ResourceObject(propValue, withProfile);
                            expandoDict[propertyInfo.Name] = LinkObject.LinkToOne(resourceObject, resRel.Sideload);
                        }
                        else
                        {
                            expandoDict[propertyInfo.Name] = LinkObject.Empty(LinkType.ToOne);
                        }
                    }
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
                if (kvp.Value == null)
                {
                    continue;
                }

                var linkObject = kvp.Value as LinkObject;
                if (linkObject != null)
                {
                    linkObject.Resources.ForEach(r => accumulatedResources.AddIgnoringDuplicates(r.ExtractAndRewireResourceLinks()));
                    accumulatedResources.AddIgnoringDuplicates(linkObject.Resources);
                    expandoDict.Remove(kvp.Key);
                    AddToLinks(kvp.Key, linkObject);
                }
            }

            return accumulatedResources;
        }

        private void AddToLinks(string linkName, LinkObject link)
        {
            var expandoDict = (IDictionary<string, object>)_innerExpando;
            if (!expandoDict.ContainsKey("Links"))
            {
                expandoDict.Add("Links", new ExpandoObject());
            }
            var links = (IDictionary<string, object>)expandoDict["Links"];
            links.Add(linkName, link);
        }

        public void WriteJson(JsonWriter writer, JsonSerializer serializer)
        {
            IContractResolver existingResolver = serializer.ContractResolver;
            serializer.ContractResolver = new ComplexAttributeFieldNameEnforcingContractResolver(existingResolver);
            serializer.Serialize(writer, _innerExpando);
            serializer.ContractResolver = existingResolver;
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

                    // Ignore LinkObjects
                    if (type == typeof(LinkObject))
                    {
                        return contract;
                    }

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

        #region Equality
        protected bool Equals(ResourceObject other)
        {
            return Equals(ResourceIdentifier, other.ResourceIdentifier);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((ResourceObject)obj);
        }

        public override int GetHashCode()
        {
            return (ResourceIdentifier != null ? ResourceIdentifier.GetHashCode() : 0);
        }

        public static bool operator ==(ResourceObject left, ResourceObject right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ResourceObject left, ResourceObject right)
        {
            return !Equals(left, right);
        }
        #endregion
    }
}
