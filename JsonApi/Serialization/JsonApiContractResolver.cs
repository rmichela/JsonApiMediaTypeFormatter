using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JsonApi.Profile;
using Newtonsoft.Json.Serialization;

namespace JsonApi.Serialization
{
    internal class JsonApiContractResolver : DefaultContractResolver
    {
        private readonly IJsonApiProfile _profile;

        public JsonApiContractResolver(IJsonApiProfile profile)
        {
            _profile = profile;
        }

        protected override string ResolvePropertyName(string propertyName)
        {
            return _profile.PropertyNameResolver.ResolvePropertyName(propertyName);
        }
    }
}
