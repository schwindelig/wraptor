using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Wraptor
{
    public class MetaInfoProvider
    {
        /// <summary>
        /// Generates/returns the key used to store and access the type cache for the given type
        /// </summary>
        public static string GetTypeIdentifier(Type type) => type.AssemblyQualifiedName;

        private static readonly ConcurrentDictionary<string, Type> TypeCache = new ConcurrentDictionary<string, Type>();

        /// <summary>
        /// Returns all MethodInfos that should be generated and caches the type.
        /// </summary>
        public static IEnumerable<MethodInfo> GetMethodInfos(Type type)
        {
            AddToTypeCache(type);
            return type.GetMethods();
        }

        /// <summary>
        /// Returns the MethodInfo of the Method at index methodIndex for the given type (provided in identifier).
        /// Requires the type to be cached, which happens when calling the "GetMethodInfos" method
        /// </summary>
        public static MethodInfo GetMethodInfo(string identifier, int methodIndex)
        {
            var type = GetFromTypeCache(identifier);
            return GetMethodInfos(type).ElementAt(methodIndex);
        }

        public static ParameterInfos GetParameterInfos(MethodInfo methodInfo) => new ParameterInfos(methodInfo.GetParameters());

        public static Type GetReturnType(MethodInfo methodInfo) => methodInfo.ReturnType;

        private static void AddToTypeCache(Type type)
        {
            var identifier = GetTypeIdentifier(type);
            if (!TypeCache.ContainsKey(identifier))
            {
                TypeCache.TryAdd(identifier, type);
            }
        }

        private static Type GetFromTypeCache(string identifier) => TypeCache[identifier];
    }

    public class ParameterInfos
    {
        public int ParameterCount => this.parameterInfos.Length;

        private readonly ParameterInfo[] parameterInfos;

        public ParameterInfos(ParameterInfo[] parameterInfos)
        {
            this.parameterInfos = parameterInfos;
        }

        public Type GetParameterType(int index) => this.parameterInfos[index].ParameterType;

        public bool IsParameterValueType(int index) => this.GetParameterType(index).IsValueType;
    }
}
