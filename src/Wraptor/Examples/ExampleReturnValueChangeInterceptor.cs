using System.Reflection;

namespace Wraptor.Examples
{
    public class ExampleReturnValueChangeInterceptor : InterceptorBase
    {
        public override object Invoke(object implementation, MethodInfo methodInfo, object[] arguments)
        {
            var returnValue = base.Invoke(implementation, methodInfo, arguments);
            if (methodInfo.ReturnType == typeof(string))
            {
                returnValue = "#~ CHANGED RETURN VALUE ~#";
            }

            return returnValue;
        }
    }
}
