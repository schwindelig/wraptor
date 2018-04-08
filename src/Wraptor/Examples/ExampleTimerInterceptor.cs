using System;
using System.Diagnostics;
using System.Reflection;

namespace Wraptor.Examples
{
    public class ExampleTimerInterceptor : InterceptorBase
    {
        public override object Invoke(object implementation, MethodInfo methodInfo, object[] arguments)
        {
            var stopWatch = Stopwatch.StartNew();
            var result = base.Invoke(implementation, methodInfo, arguments);
            stopWatch.Stop();

            Console.WriteLine($"{methodInfo.Name} on {implementation.GetType().FullName} took {stopWatch.ElapsedMilliseconds}ms");

            return result;
        }
    }
}
