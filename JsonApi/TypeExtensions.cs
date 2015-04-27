using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonApi
{
    internal static class TypeExtensions
    {
        public static IEnumerable<Type> GetGenericIEnumerables(this Type type)
        {
            Type[] interfaces = type.IsInterface ? new[] {type} : type.GetInterfaces();
            var enumerableInterfaces = interfaces.Where(t => t.IsGenericType
                                            && t.GetGenericTypeDefinition() == typeof (IEnumerable<>));
            var genaricArguments = enumerableInterfaces.Select(t => t.GetGenericArguments()[0]);
            return genaricArguments;
        }

        public static void AddIgnoringDuplicates<T>(this HashSet<T> hashSet, IEnumerable<T> enumerable)
        {
            foreach (var e in enumerable)
            {
                hashSet.Add(e);
            }
        }
    }
}
