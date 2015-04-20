using System;
using System.Collections.Generic;
using System.Linq;
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
            Assert.AreEqual("foo", "foo");
        }
    }
}
