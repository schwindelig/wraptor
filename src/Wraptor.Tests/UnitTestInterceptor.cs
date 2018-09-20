using System.Reflection;

namespace Wraptor.Tests
{
    public class UnitTestInterceptor : InterceptorBase
    {
        public bool PreInvokeHit { get; set; }

        public bool InvokeHit { get; set; }

        public bool PostInvokeHit { get; set; }

        public override void PreInvoke(object implementation, MethodInfo methodInfo, object[] arguments)
        {
            this.PreInvokeHit = true;
            base.PreInvoke(implementation, methodInfo, arguments);
        }

        public override object Invoke(object implementation, MethodInfo methodInfo, object[] arguments)
        {
            this.InvokeHit = true;

            return base.Invoke(implementation, methodInfo, arguments);
        }

        public override void PostInvoke(object implementation, MethodInfo methodInfo, object[] arguments, object returnValue)
        {
            this.PostInvokeHit = true;
            base.PostInvoke(implementation, methodInfo, arguments, returnValue);
        }
    }
}
