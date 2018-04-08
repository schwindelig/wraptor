using System;
using System.Reflection;

namespace Wraptor.Examples
{
    public class ExampleLoggerInterceptor : InterceptorBase
    {
        public override void PreInvoke(object implementation, MethodInfo methodInfo, object[] arguments)
        {
            Console.WriteLine($"Pre-Invoke for {methodInfo.Name} on {implementation}");
            Console.WriteLine($"> Number of arguments: {arguments.Length}");
            for (var i = 0; i < arguments.Length; i++)
            {
                Console.WriteLine($"  arg[{i}]: {arguments[i]}");
            }
        }

        public override object Invoke(object implementation, MethodInfo methodInfo, object[] arguments)
        {
            Console.WriteLine($"Invoking {methodInfo.Name} on {implementation}");

            var returnValue = methodInfo.Invoke(implementation, arguments);

            Console.WriteLine($"Invoked {methodInfo.Name} on {implementation}");

            return returnValue;
        }

        public override void PostInvoke(object implementation, MethodInfo methodInfo, object[] arguments, object returnValue)
        {
            Console.WriteLine($"Post-Invoke for {methodInfo.Name} on {implementation}");
            var returnType = methodInfo.ReturnType;
            if (returnType != typeof(void))
            {
                Console.WriteLine($"> returned {returnValue} ({returnType.FullName})");
            }

            Console.WriteLine(Environment.NewLine);
        }
    }
}
