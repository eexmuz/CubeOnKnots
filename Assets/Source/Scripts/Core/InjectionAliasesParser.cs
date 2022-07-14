using System;
using System.Collections.Generic;
using System.Reflection;
using Core.Attributes;

namespace Core
{
    public static class InjectionAliasesParser
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Appends the injection map to the existing one.
        /// </summary>
        /// <param name="objects">The list of processed objects.</param>
        /// <param name="map">The Map to append results to.</param>
        /// <param name="useObjectsTypeAsDefault">
        ///     If set to <c>true</c> use objects type as the injection alias if there is no
        ///     InjectionAlias attribute.
        /// </param>
        public static void AppendInjectionMap(IEnumerable<object> objects, IDictionary<Type, object> map,
            bool useObjectsTypeAsDefault = false)
        {
            foreach (var obj in objects)
            {
                var objType = obj.GetType();
                MemberInfo info = objType;
                var attributes = info.GetCustomAttributes(true);

                var hasAliasAttribute = false;
                foreach (var attribute in attributes)
                {
                    var injectionAlias = attribute as InjectionAlias;
                    if (injectionAlias != null)
                    {
                        if (!injectionAlias.AliasType.IsAssignableFrom(objType))
                            throw new Exception(string.Format("Alias type '{0}' is not assignable from '{1}'",
                                injectionAlias.AliasType.FullName, objType.FullName));
                        if (map.ContainsKey(injectionAlias.AliasType))
                            throw new Exception(string.Format(
                                "Alias type '{0}' is duplicated in classes '{1}' and '{2}'",
                                injectionAlias.AliasType.FullName,
                                map[injectionAlias.AliasType].GetType().FullName,
                                objType.FullName
                                ));
                        hasAliasAttribute = true;
                        map[injectionAlias.AliasType] = obj;
                        break;
                    }
                }

                if (useObjectsTypeAsDefault && !hasAliasAttribute)
                    map.Add(objType, obj);
            }
        }

        /// <summary>
        ///     Generates the new injection map.
        /// </summary>
        /// <returns>The injection map.</returns>
        /// <param name="objects">The list of processed objects.</param>
        /// <param name="useObjectsTypeAsDefault">
        ///     If set to <c>true</c> use objects type as the injection alias if there is no
        ///     InjectionAlias attribute.
        /// </param>
        public static IDictionary<Type, object> GenerateInjectionMap(IEnumerable<object> objects,
            bool useObjectsTypeAsDefault = false)
        {
            IDictionary<Type, object> res = new Dictionary<Type, object>();
            AppendInjectionMap(objects, res, useObjectsTypeAsDefault);
            return res;
        }

        #endregion
    }
}