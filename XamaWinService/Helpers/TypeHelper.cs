using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XamaWinService.Helpers
{
    public static class TypeHelper
    {
        public static IList<Type> GetTypesExtending(this Type type, string assemblyNamePart, bool excludeAbstract = false, bool excludeInterface = false)
        {
            var rs = new List<Type>();
            var asl = AppDomain.CurrentDomain.GetAssemblies();
            if (!string.IsNullOrEmpty(assemblyNamePart))
            {
                asl = asl.Where(p => p.FullName.Contains(assemblyNamePart)).ToArray();
            }
            Assembly @as;
            for (int i = 0; i < asl.Length; i++)
            {
                @as = asl[i];
                foreach (var t in @as.GetTypes())
                {
                    if (t.IsInterface && excludeInterface) continue;
                    if (t.IsAbstract && excludeAbstract) continue;
                    if (type.IsAssignableFrom(t) && rs.Find(p => p.Equals(t)) == null)
                    {
                        rs.Add(t);
                    }
                    if (type.IsInterface)
                    {
                        foreach (var intr in t.GetInterfaces())
                        {
                            if (intr.IsGenericType && intr.GetGenericTypeDefinition() == type)
                            {
                                rs.Add(t);
                                break;
                            }
                        }
                    }
                }
            }
            return rs;
        }

        /// <summary>
        /// find <see cref="Type">'s that extends the parameter type
        /// </summary>
        /// <typeparam name="T"><see cref="Type"> to find</typeparam>
        /// <param name="excludeAbstract"> should ignore abstract classes</param>
        /// <param name="excludeInterface">should ignore interfaces</param>
        /// <returns>list of types that extends the desired type</returns>
        public static IList<Type> GetTypesExtending<T>(bool excludeAbstract = false, bool excludeInterface = false)
        {
            return GetTypesExtending<T>(null, excludeAbstract, excludeInterface);
        }

        public static IList<Type> GetTypesExtending<T>(string assemblyNamePart, bool excludeAbstract = false, bool excludeInterface = false)
        {
            return GetTypesExtending(typeof(T), assemblyNamePart, excludeAbstract, excludeInterface);
        }

    }
}