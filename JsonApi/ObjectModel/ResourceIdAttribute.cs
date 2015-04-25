﻿using System;

namespace JsonApi.ObjectModel
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ResourceIdAttribute : Attribute
    {
    }
}
