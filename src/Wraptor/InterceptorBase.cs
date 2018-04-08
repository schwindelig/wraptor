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
            object result = null;
            if (implementation != null && methodInfo != null)
            {
               result = methodInfo.Invoke(implementation, arguments);
            }

            return result;
        }

        public virtual void PostInvoke(object implementation, MethodInfo methodInfo, object[] arguments, object returnValue)
        {
        }
    }
}
