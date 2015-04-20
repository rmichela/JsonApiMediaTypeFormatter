using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace JsonApi
{
    internal static class DynamicExtensions
    {
        public static dynamic InitializeExpandoFromPublicObjectProperties(object o)
        {
            Type t = o.GetType();
            PropertyInfo[] objectProperties = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            dynamic expando = new ExpandoObject();
            IDictionary<string, object> expandoDict = expando;

            foreach (PropertyInfo objectProperty in objectProperties)
            {
                expandoDict[objectProperty.Name] = objectProperty.GetValue(o);
            }

            return expando;
        }
    }
}
