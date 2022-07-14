using System;

namespace Core.Attributes
{
    /// <summary>
    ///     Attribute class used to provide a decoration for injectable properties
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class Inject : Attribute
    {
    }
}