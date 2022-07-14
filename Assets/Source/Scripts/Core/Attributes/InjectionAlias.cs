using System;

namespace Core.Attributes
{
    /// <summary>
    ///     Injection alias attribute.
    ///     Mark the class with this attribute, passing the type you want it to be linked to as an argument.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class InjectionAlias : Attribute
    {
        #region Fields

        public readonly Type AliasType;

        #endregion

        #region Constructors and Destructors

        public InjectionAlias(Type aliasType)
        {
            AliasType = aliasType;
        }

        #endregion
    }
}