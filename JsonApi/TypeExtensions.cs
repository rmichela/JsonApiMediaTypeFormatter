using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace JsonApi
{
    internal static class TypeExtensions
    {
        public const BindingFlags PUBLIC_INSTANCE = BindingFlags.Public | BindingFlags.Instance;

        public static IEnumerable<Type> GetGenericIEnumerables(this Type type)
        {
            Type[] interfaces = type.IsInterface ? new[] {type} : type.GetInterfaces();
            var enumerableInterfaces = interfaces.Where(t => t.IsGenericType
                                            && t.GetGenericTypeDefinition() == typeof (IEnumerable<>));
            var genaricArguments = enumerableInterfaces.Select(t => t.GetGenericArguments()[0]);
            return genaricArguments;
        }

        public static PropertyFieldInfo[] GetPropertiesAndFields(this Type type, BindingFlags bindingFlags)
        {
            var properties = type.GetProperties(bindingFlags).Select(p => new PropertyFieldInfo(p));
            var fields = type.GetFields(bindingFlags).Select(f => new PropertyFieldInfo(f));
            return properties.Union(fields).ToArray();
        }

        public static PropertyFieldInfo GetPropertyOrField(this Type type, string name, BindingFlags bindingFlags)
        {
            var property = type.GetProperty(name, bindingFlags);
            if (property != null) return new PropertyFieldInfo(property);

            var field = type.GetField(name, bindingFlags);
            if (field != null) return new PropertyFieldInfo(field);

            return null;
        }

        public static void AddIgnoringDuplicates<T>(this HashSet<T> hashSet, IEnumerable<T> enumerable)
        {
            foreach (var e in enumerable)
            {
                hashSet.Add(e);
            }
        }

        public static dynamic InitializeExpandoFromPublicObjectProperties(object o)
        {
            Type t = o.GetType();
            PropertyFieldInfo[] objectProperties = t.GetPropertiesAndFields(PUBLIC_INSTANCE);
            dynamic expando = new ExpandoObject();
            IDictionary<string, object> expandoDict = expando;

            foreach (PropertyFieldInfo propertyOrField in objectProperties)
            {
                expandoDict[propertyOrField.Name] = propertyOrField.GetValue(o);
            }

            return expando;
        }
    }
}
