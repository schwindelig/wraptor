using System.Reflection;

namespace Wraptor
{
    public interface IInterceptor
    {
        void PreInvoke(object implementation, MethodInfo methodInfo, object[] arguments);

        object Invoke(object implementation, MethodInfo methodInfo, object[] arguments);

        void PostInvoke(object implementation, MethodInfo methodInfo, object[] arguments, object returnValue);
    }
}
