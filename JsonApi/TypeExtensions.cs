using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonApi
{
    internal static class TypeExtensions
    {
        public static IEnumerable<Type> GetGenericIEnumerables(this Type type)
        {
            return type
                    .GetInterfaces()
                    .Where(t => t.IsGenericType
                        && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .Select(t => t.GetGenericArguments()[0]);
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
