using Xunit;

namespace Wraptor.Tests
{
    public class ProxyGenericTests
    {
        [Fact]
        public void MethodWithGenericReturnValue_IsInvokable()
        {
            var generator = new Generator();
            var interceptor = new UnitTestInterceptor();
            var originalInstance = new SampleClass();
            var proxy = generator.Generate<ISampleClass>(originalInstance, interceptor);

            var result = proxy.GetT("Foo");

            Assert.Equal("Foo", result);
            Assert.True(interceptor.PreInvokeHit);
            Assert.True(interceptor.InvokeHit);
            Assert.True(interceptor.PostInvokeHit);
        }

        [Fact]
        public void MethodWithMultipleGenericParameters_IsInvokable()
        {
            var generator = new Generator();
            var interceptor = new UnitTestInterceptor();
            var originalInstance = new SampleClass();
            var proxy = generator.Generate<ISampleClass>(originalInstance, interceptor);

            var result1 = proxy.GetT1("Foo", 42);
            var result2 = proxy.GetT2("Foo", 42);

            Assert.Equal("Foo", result1);
            Assert.Equal(42, result2);
            Assert.True(interceptor.PreInvokeHit);
            Assert.True(interceptor.InvokeHit);
            Assert.True(interceptor.PostInvokeHit);
        }

        [Fact]
        public void MethodWithGenericCovariantReturnType_IsInvokable()
        {
            var generator = new Generator();
            var interceptor = new UnitTestInterceptor();
            var originalInstance = new SampleGenericClass();
            var proxy = generator.Generate<ISampleGenericClass<int, string>>(originalInstance, interceptor);

            var result = proxy.GetCovariantResult(42);

            Assert.Equal("42", result);
            Assert.True(interceptor.PreInvokeHit);
            Assert.True(interceptor.InvokeHit);
            Assert.True(interceptor.PostInvokeHit);
        }
    }
}
