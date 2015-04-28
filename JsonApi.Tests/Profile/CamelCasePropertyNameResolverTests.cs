using JsonApi.Profile;
using NUnit.Framework;

namespace JsonApi.Tests.Profile
{
    [TestFixture]
    public class CamelCasePropertyNameResolverTests
    {
        [Test]
        public void ResolverShouldCamelCase()
        {
            var resolver = new CamelCasePropertyNameResolver();
            Assert.AreEqual("camelCase", resolver.ResolvePropertyName("CamelCase"));
        }
    }
}
