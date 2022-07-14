using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Attributes;
using Core.Exceptions;

namespace Core
{
    /// <summary>
    ///     Mapping resolver delegate.
    ///     Returns an instance of the object, which is mapped to the 'type' or null if mapping for current type is not set.
    /// </summary>
    public delegate object MappingResolverDelegate(Type type);

    /// <summary>
    ///     Supplies dependencies injections using type mapping resolver delegate
    /// </summary>
    public class Injector
    {
        #region Fields

        /// <summary>
        ///     Injectable fields cache.
        ///     Used to optimize fields injection.
        /// </summary>
        private readonly Dictionary<Type, List<FieldInfo>> _cachedFieldsForType;

        /// <summary>
        ///     Injectable properties cache.
        ///     Used to optimize properties injection.
        /// </summary>
        private readonly Dictionary<Type, List<PropertyInfo>> _cachedPropertiesForType;

        private readonly MappingResolverDelegate _mappingResolver;

        #endregion

        #region Constructors and Destructors

        public Injector(MappingResolverDelegate mappingResolver)
        {
            _mappingResolver = mappingResolver;
            _cachedFieldsForType = new Dictionary<Type, List<FieldInfo>>();
            _cachedPropertiesForType = new Dictionary<Type, List<PropertyInfo>>();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Perform injection on the instance using MappingResolverDelegate
        /// </summary>
        public void Inject(object instance, bool shouldThrowException = true)
        {
            InjectProperties(instance, shouldThrowException);
            InjectFields(instance, shouldThrowException);
        }

        /// <summary>
        ///     Perform injecton on the instance using values from arguments
        /// </summary>
        public void Inject(object instance, object[] args)
        {
            var cachedProps = GetInjectableProps(instance);
            var cachedFields = GetInjectableFields(instance);
            if (cachedProps == null && cachedFields == null)
                return;
            foreach (var val in args)
            {
                var valType = val.GetType();
                if (cachedProps != null)
                    foreach (var prop in cachedProps)
                        if (prop.PropertyType == valType)
                            prop.SetValue(instance, val, null);
                if (cachedFields != null)
                    foreach (var field in cachedFields)
                        if (field.FieldType == valType)
                            field.SetValue(instance, val);
            }
        }

        public void InjectFields(object instance, bool shouldThrowException = true)
        {
            var cachedFields = GetInjectableFields(instance);
            if (cachedFields == null)
                return;
            foreach (var field in cachedFields)
                try
                {
                    var value = _mappingResolver(field.FieldType);
                    if (value == null)
                        throw new InjectionException("Could not resolve value for type: " + field.FieldType);

                    field.SetValue(instance, value);
                }
                catch (Exception ex)
                {
                    if (shouldThrowException)
                        throw new InjectionException(string.Format("Injection Error: Can't inject '{0}' into '{1}'",
                            field.FieldType, instance.GetType()), ex);
                }
        }

        public void InjectProperties(object instance, bool shouldThrowException = true)
        {
            var cachedProps = GetInjectableProps(instance);
            if (cachedProps == null)
                return;
            foreach (var prop in cachedProps)
                try
                {
                    var value = _mappingResolver(prop.PropertyType);
                    if (value == null)
                        throw new InjectionException("Could not resolve value for type: " + prop.PropertyType);
                    prop.SetValue(instance, value, null);
                }
                catch (Exception ex)
                {
                    if (shouldThrowException)
                        throw new InjectionException(string.Format("Injection Error: Can't inject '{0}' into '{1}'",
                            prop.PropertyType, instance.GetType()), ex);
                }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the injectable fields of the instance.
        /// </summary>
        private List<FieldInfo> GetInjectableFields(object instance)
        {
            var type = instance.GetType();
            List<FieldInfo> cachedFields;
            if (_cachedFieldsForType.TryGetValue(type, out cachedFields))
            {
                if (cachedFields == null)
                    return null;
            }
            else
            {
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var field in fields)
                {
                    var attrs = field.GetCustomAttributes(true);
                    foreach (var attr in attrs)
                    {
                        var injectAttr = attr as Inject;
                        if (injectAttr != null)
                        {
                            if (cachedFields == null)
                                cachedFields = new List<FieldInfo>();
                            cachedFields.Add(field);
                        }
                    }
                }

                _cachedFieldsForType[type] = cachedFields;
            }

            return cachedFields;
        }

        /// <summary>
        ///     Gets the injectable properties of the instance.
        /// </summary>
        private List<PropertyInfo> GetInjectableProps(object instance)
        {
            var type = instance.GetType();
            List<PropertyInfo> cachedProps;
            if (_cachedPropertiesForType.TryGetValue(type, out cachedProps))
            {
                if (cachedProps == null)
                    return null;
            }
            else
            {
                var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                foreach (var prop in props)
                {
                    var attrs = prop.GetCustomAttributes(true);
                    foreach (var attr in attrs)
                    {
                        var injectAttr = attr as Inject;
                        if (injectAttr != null)
                        {
                            if (cachedProps == null)
                                cachedProps = new List<PropertyInfo>();
                            cachedProps.Add(prop);
                        }
                    }
                }

                _cachedPropertiesForType[type] = cachedProps;
            }

            return cachedProps;
        }

        #endregion
    }
}