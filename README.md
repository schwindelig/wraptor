# Wraptor

**A simple dynamic proxy targeting .net standard 2.0**

[![Build status](https://ci.appveyor.com/api/projects/status/nobrrdmb88sdkcht?svg=true)](https://ci.appveyor.com/project/DavidSzke/wraptor)

Wraptor allows you to extend, intercept, modify and redirect calls to interface defined methods on any object. This is achieved by generating a proxy object at run-time that redirects calls to an object of type `IInterceptor`.

`IInterceptor` currently provides three hooks:
- `void PreInvoke(object implementation, MethodInfo methodInfo, object[] arguments)`
- `object Invoke(object implementation, MethodInfo methodInfo, object[] arguments)`
- `void PostInvoke(object implementation, MethodInfo methodInfo, object[] arguments, object returnValue)`

Wraptor's IL-Generation and proxy source is based on on John Mikhail's article [Dynamic Proxy Creation Using C# Emit](https://www.codeproject.com/Articles/5511/Dynamic-Proxy-Creation-Using-C-Emit)

## Getting started
First, you need to define an implementation of `Wraptor.IInterceptor`. Wraptor provides a handy `Wraptor.InterceptorBase` class you can extend:
```csharp
public class MyInterceptor : InterceptorBase
{
    public override void PreInvoke(object implementation, MethodInfo methodInfo, object[] arguments)
    {
        Console.WriteLine($"PreInvoke: {methodInfo.Name} on {implementation.GetType().Name}");
        base.PreInvoke(implementation, methodInfo, arguments);
    }

    public override object Invoke(object implementation, MethodInfo methodInfo, object[] arguments)
    {
        Console.WriteLine($"Invoke: {methodInfo.Name} on {implementation.GetType().Name}");
        return base.Invoke(implementation, methodInfo, arguments);
    }

    public override void PostInvoke(object implementation, MethodInfo methodInfo, object[] arguments, object returnValue)
    {
        Console.WriteLine($"PostInvoke: {methodInfo.Name} on {implementation.GetType().Name}");
        base.PostInvoke(implementation, methodInfo, arguments, returnValue);
    }
}
```
This implementation can now be used on an instance of the `Wraptor.Generator` class to create the proxy using the `Generate<T>` method:
```csharp
public interface IPerson
{
    void SayName();
}

public class Person : IPerson
{
    public string Name { get; set; }

    public void SayName() => Console.WriteLine($"Hi, my name is {this.Name}");
}

public class Program
{
    static void Main(string[] args)
    {
        var generator = new Generator();
        var interceptor = new MyInterceptor();
        var alice = new Person { Name = "Alice" };
        var proxy = generator.Generate<IPerson>(alice, interceptor);

        proxy.SayName();
    }
}
```
Which will result in the following output:
```
PreInvoke: SayName on Person
Invoke: SayName on Person
Hi, my name is Alice
PostInvoke: SayName on Person
```
Please note the explicit definition of `IPerson` in the `Generate<T>` call. Wraptor uses the here defined interface to generate the proxy methods. Defining anything other than an interface will result in an `ArgumentException`.

## Usage examples
### Measuring a method's execution time
One way is to start a `Stopwatch` in the `Invoke` method, wait for the actual method to complete and then process the ellapsed time.
```csharp
public class MeasureInterceptor : InterceptorBase
{
    public override object Invoke(object implementation, MethodInfo methodInfo, object[] arguments)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = base.Invoke(implementation, methodInfo, arguments);
        Console.WriteLine($"{methodInfo.Name} took {stopwatch.ElapsedMilliseconds}ms");

        return result;
    }
}
```
Another approach could be to have a timer at instance level of the Interceptor and toggle the measuring in the `PreInvoke` and `PostInvoke` methods.
### Modifying arguments passed to the method

```csharp
public interface IPerson
{
    void Deposit(decimal moneys);
}

public class Person : IPerson
{
    public decimal Balance { get; set; }

    public void Deposit(decimal moneys) => this.Balance += moneys;
}

public class ArgumentInterceptor : InterceptorBase
{
    public override void PreInvoke(object implementation, MethodInfo methodInfo, object[] arguments)
    {
        if (methodInfo.Equals(typeof(IPerson).GetMethod(nameof(IPerson.Deposit))))
        {
            arguments[0] = 90001m;
        }
    }
}

public class Program
{
    static void Main(string[] args)
    {
        var generator = new Generator();
        var interceptor = new ArgumentInterceptor();
        var alice = new Person { Balance = 0 };
        var proxy = generator.Generate<IPerson>(alice, interceptor);

        proxy.Deposit(42);
        Console.WriteLine(alice.Balance);
    }
}
```
### Modifying the return value
### Surpressing method invocations
### Redirecting method invocations
### Chaining interceptor

## Implementation details
- Information Provider
- IL generation

## Roadmap
- Support for non-interface public methods
- Interception for private method calls (not sure if this is even possible)
- Support for extracting and saving generated proxies
