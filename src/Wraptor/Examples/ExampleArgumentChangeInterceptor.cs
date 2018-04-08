using System.Linq;
using System.Reflection;

namespace Wraptor.Examples
{
    public class ExampleArgumentChangeInterceptor : InterceptorBase
    {
        public override void PreInvoke(object implementation, MethodInfo methodInfo, object[] arguments)
        {
            if (arguments == null || !arguments.Any()) return;

            if (arguments[0] is string)
            {
                arguments[0] = "#~ CHANGED ARGUMENT ~#";
            }
        }
    }
}
