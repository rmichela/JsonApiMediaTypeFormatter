using System;
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
        public void ExpandosShouldDoSomething()
        {
            var x = new Tuple<string, int>("A", 10);
            dynamic d = InitializeExpandoFromObjectProperties(x);
            Console.WriteLine(d.Item1);
            Console.WriteLine(d.Item2);

            d.Item1 = 42;
            Console.WriteLine(d.Item1);

            IEnumerable<string> y = new[] { "bananas" };
        }

        private dynamic InitializeExpandoFromObjectProperties(object o)
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
