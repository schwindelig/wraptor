using System;
using System.Collections.Generic;
using System.Reflection;

namespace Wraptor
{
    public abstract class InterceptorBase : IInterceptor
    {
        public virtual void PreInvoke(object implementation, MethodInfo methodInfo, object[] arguments)
        {
        }

        public virtual object Invoke(object implementation, MethodInfo methodInfo, object[] arguments)
        {
            if (implementation == null || methodInfo == null) return null;

            // TODO: We might move this to MetaInfoProvider so we don't have to do this for every method invocation
            object result = null;
            if (methodInfo.IsGenericMethod)
            {
                var genericParameterTypes = new List<Type>();

                var parameters = methodInfo.GetParameters();
                for (var i = 0; i < arguments.Length; i++)
                {
                    var parameter = parameters[i];

                    if (!parameter.ParameterType.IsGenericParameter) continue;

                    var argumentType = arguments[i].GetType();
                    genericParameterTypes.Add(argumentType);
                }

                methodInfo = methodInfo.MakeGenericMethod(genericParameterTypes.ToArray());
            }

            result = methodInfo.Invoke(implementation, arguments);

            return result;
        }

        public virtual void PostInvoke(object implementation, MethodInfo methodInfo, object[] arguments, object returnValue)
        {
        }
    }
}
