using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace JsonApi
{
    internal class JsonApiContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Restrict serialization only to Properties
        /// </summary>
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            List<MemberInfo> allMembers = base.GetSerializableMembers(objectType);
            List<MemberInfo> filteredMembers = allMembers.Where(m => m.MemberType == MemberTypes.Property).ToList();
            return filteredMembers;
        }
    }
}
