using System;

namespace DapperParameters.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnorePropertyAttribute : Attribute
    {
    }
}
