using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace JsonApi.Tests
{
    [TestFixture]
    public class ScratchTests
    {
        [Test]
        public void Foo()
        {
            var array = new int[] { 1, 2, 3 };
            var list = new List<int> { 1, 2, 3 };
            var set = new HashSet<int>{ 1, 2, 3 };
            var sequence = YieldInt.Sequence();

            var enumerableArray = new object[] { array, list, set, sequence };
            foreach (IEnumerable enumerable in enumerableArray)
            {
                foreach (object i in enumerable)
                {
                    Console.Write(i);
                }
                Console.WriteLine();
            }

            foreach (object enumerable in enumerableArray)
            {
                var t = enumerable.GetType();
                if (typeof(IEnumerable).IsAssignableFrom(t))
                {
                    Console.WriteLine(GetGenericIEnumerables(enumerable).First());
                }
            }
        }

        [Test]
        public void Foo2()
        {
            Console.WriteLine(GetGenericIEnumerables(42).Count());
        }

        public IEnumerable<Type> GetGenericIEnumerables(object o)
        {
            return o.GetType()
                    .GetInterfaces()
                    .Where(t => t.IsGenericType
                        && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .Select(t => t.GetGenericArguments()[0]);
        }

        private static class YieldInt
        {
            public static IEnumerable Sequence()
            {
                yield return 1;
                yield return 2;
                yield return 3;
            }
        }
    }
}
