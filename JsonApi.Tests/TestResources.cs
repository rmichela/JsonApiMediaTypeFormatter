using System.Collections.Generic;
using JsonApi.ObjectModel;

namespace JsonApi.Tests
{
    [ResourceObject]
    public class ResourceWithRelationship
    {
        public int Id { get; set; }
        [ResourceRelationship(Sideload = true)]
        public Resource ToOne { get; set; }
        [ResourceRelationship(Sideload = true)]
        public List<Resource> ToMany { get; set; }
    }

    [ResourceObject]
    public class ResourceWithRelationshipNoSideload
    {
        public int Id { get; set; }
        [ResourceRelationship(Sideload = false)]
        public Resource ToOne { get; set; }
        [ResourceRelationship(Sideload = false)]
        public List<Resource> ToMany { get; set; }
    }

    [ResourceObject]
    public class ResourceWithConflictingRelationship
    {
        public int Id { get; set; }
        [ResourceRelationship(Sideload = false)]
        public Resource Conflict { get; set; }
        public int CoNfLiCt { get; set; }
    }

    [ResourceObject]
    public class Resource
    {
        public int Id { get; set; }
        public string AttributeS { get; set; }
        public int AttributeI { get; set; }
    }

    [ResourceObject(Type = "Bananas")]
    public class ResourceWithAlternateType
    {
        public int Id { get; set; }
    }

    public class ResourceWithoutAttribute
    {
        public int Id { get; set; }
    }

    [ResourceObject]
    public class ResourceWithoutId
    {
        public int Value { get; set; }
    }

    [ResourceObject]
    public class ResourceWithoutIdWithAttribute
    {
        [ResourceId]
        public int Value { get; set; }
    }

    [ResourceObject]
    public class ResourceWithComplexAttribute
    {
        public int Id { get; set; }
        public ComplexAttribute AttributeC { get; set; }
    }

    public class ComplexAttribute
    {
        public string AttributeS { get; set; }
        public int AttributeI { get; set; }
    }

    [ResourceObject]
    public class ResourceWithIllegalComplexAttributes
    {
        public int Id { get; set; }
        public ComplexAttributeWithId WithId { get; set; }
        public ComplexAttributeWithType WithType { get; set; }
        public ComplexAttributeWithLinks WithLinks { get; set; }
        public ComplexAttributeWithMeta WithMeta { get; set; }
        public ComplexAttributeWithIllegalAttribute WithNested { get; set; }
    }

    public class ComplexAttributeWithId
    {
        public int Id { get; set; }
    }

    public class ComplexAttributeWithType
    {
        public int Type { get; set; }
    }

    public class ComplexAttributeWithLinks
    {
        public int Links { get; set; }
    }

    public class ComplexAttributeWithMeta
    {
        public int Meta { get; set; }
    }

    public class ComplexAttributeWithIllegalAttribute
    {
        public ComplexAttributeWithId WithId { get; set; }
    }

    [ResourceObject]
    public class ResourceWithSelf
    {
        public int Id { get; set; }
        [ResourceRelationship]
        public int Self { get; set; }
    }

    [ResourceObject]
    public class ResourceWithType
    {
        public int Id { get; set; }
        public int Type { get; set; }
    }

    [ResourceObject]
    public class ResourceWithLinks
    {
        public int Id { get; set; }
        public int Links { get; set; }
    }

    [ResourceObject]
    public class ResourceWithMeta
    {
        public int Id { get; set; }
        public int Meta { get; set; }
    }
}
