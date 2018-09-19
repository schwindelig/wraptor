using System.Reflection;

namespace Wraptor.Tests
{
    public class UnitTestInterceptor : IInterceptor
    {
        public bool PreInvokeHit { get; set; }

        public bool InvokeHit { get; set; }

        public bool PostInvokeHit { get; set; }

        public void PreInvoke(object implementation, MethodInfo methodInfo, object[] arguments)
        {
            this.PreInvokeHit = true;
        }

        public object Invoke(object implementation, MethodInfo methodInfo, object[] arguments)
        {
            this.InvokeHit = true;

            return methodInfo.Invoke(implementation, arguments);
        }

        public void PostInvoke(object implementation, MethodInfo methodInfo, object[] arguments, object returnValue)
        {
            this.PostInvokeHit = true;
        }
    }
}
