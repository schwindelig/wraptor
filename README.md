# Wraptor

**A simple dynamic proxy targeting .net standard 2.0**

[![Build status](https://ci.appveyor.com/api/projects/status/nobrrdmb88sdkcht?svg=true)](https://ci.appveyor.com/project/DavidSzke/wraptor)

Wraptor allows you to extend, intercept and modify calls to interface defined methods on any object. This is achieved by generating a proxy object at run-time that redirects calls to an object of type `IInterceptor`.

`IInterceptor` currently provides three hooks:
- `void PreInvoke(object implementation, MethodInfo methodInfo, object[] arguments)`
- `object Invoke(object implementation, MethodInfo methodInfo, object[] arguments)`
- `void PostInvoke(object implementation, MethodInfo methodInfo, object[] arguments, object returnValue)`

Wraptor's IL-Generation and proxy source is based on on John Mikhail's article [Dynamic Proxy Creation Using C# Emit](https://www.codeproject.com/Articles/5511/Dynamic-Proxy-Creation-Using-C-Emit)

