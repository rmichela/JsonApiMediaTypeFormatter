using JsonApi.Profile;
using NUnit.Framework;

namespace JsonApi.Tests.Profile
{
    [TestFixture]
    public class DasherizingPropertyNameResolverTests
    {
        [Test]
        public void ShouldDasherizePascalCase()
        {
            var resolver = new DasherizingPropertyNameResolver();
            Assert.AreEqual("foo-bar-cheese", resolver.ResolvePropertyName("FooBarCheese"));
        }

        [Test]
        public void ShouldDasherizeCamelCase()
        {
            var resolver = new DasherizingPropertyNameResolver();
            Assert.AreEqual("foo-bar-cheese", resolver.ResolvePropertyName("fooBarCheese"));
        }

        [Test]
        public void ShouldDasherizeLastCharUcase()
        {
            var resolver = new DasherizingPropertyNameResolver();
            Assert.AreEqual("prop-a", resolver.ResolvePropertyName("PropA"));
        }

        [Test]
        public void ShouldNotSplitUcaseAcronyms()
        {
            var resolver = new DasherizingPropertyNameResolver();
            Assert.AreEqual("for-nasa-adventure", resolver.ResolvePropertyName("ForNASAAdventure"));            
        }

        [Test]
        public void ShouldHandleSingleCharacterUpcase()
        {
            var resolver = new DasherizingPropertyNameResolver();
            Assert.AreEqual("a", resolver.ResolvePropertyName("A"));            
        }

        [Test]
        public void ShouldHandleSingleCharacterLcase()
        {
            var resolver = new DasherizingPropertyNameResolver();
            Assert.AreEqual("a", resolver.ResolvePropertyName("a"));            
        }

        [Test]
        public void ShouldHandleTwoCharacterUpperUpper()
        {
            var resolver = new DasherizingPropertyNameResolver();
            Assert.AreEqual("ab", resolver.ResolvePropertyName("AB"));            
        }

        [Test]
        public void ShouldHandleTwoCharacterUpperLower()
        {
            var resolver = new DasherizingPropertyNameResolver();
            Assert.AreEqual("ab", resolver.ResolvePropertyName("Ab"));            
        }

        [Test]
        public void ShouldHandleTwoCharacterLowerUpper()
        {
            var resolver = new DasherizingPropertyNameResolver();
            Assert.AreEqual("a-b", resolver.ResolvePropertyName("aB"));
        }

        [Test]
        public void ShouldHandleTwoCharacterLowerLower()
        {
            var resolver = new DasherizingPropertyNameResolver();
            Assert.AreEqual("ab", resolver.ResolvePropertyName("ab"));
        }
    }
}
