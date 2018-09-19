using Xunit;

namespace Wraptor.Tests
{

    public class ProxyTests
    {
        [Fact]
        public void MethodWithIntegerReturnValue_IsInvokable()
        {
            var generator = new Generator();
            var interceptor = new UnitTestInterceptor();
            var originalInstance = new SampleClass();
            var proxy = generator.Generate<ISampleClass>(originalInstance, interceptor);

            var result = proxy.GetInt(42);

            Assert.Equal(42, result);
            Assert.True(interceptor.PreInvokeHit);
            Assert.True(interceptor.InvokeHit);
            Assert.True(interceptor.PostInvokeHit);
        }

        [Fact]
        public void MethodWithStringReturnValue_IsInvokable()
        {
            var generator = new Generator();
            var interceptor = new UnitTestInterceptor();
            var originalInstance = new SampleClass();
            var proxy = generator.Generate<ISampleClass>(originalInstance, interceptor);

            var result = proxy.GetString("Foo");

            Assert.Equal("Foo", result);
            Assert.True(interceptor.PreInvokeHit);
            Assert.True(interceptor.InvokeHit);
            Assert.True(interceptor.PostInvokeHit);
        }

        [Fact]
        public void PropertyWithIntegerType_IsInvokable()
        {
            var generator = new Generator();
            var interceptor = new UnitTestInterceptor();
            var originalInstance = new SampleClass();
            var proxy = generator.Generate<ISampleClass>(originalInstance, interceptor);

            proxy.IntProperty = 42;

            Assert.Equal(42, proxy.IntProperty);
            Assert.True(interceptor.PreInvokeHit);
            Assert.True(interceptor.InvokeHit);
            Assert.True(interceptor.PostInvokeHit);
        }

        [Fact]
        public void PropertyWithStringType_IsInvokable()
        {
            var generator = new Generator();
            var interceptor = new UnitTestInterceptor();
            var originalInstance = new SampleClass();
            var proxy = generator.Generate<ISampleClass>(originalInstance, interceptor);

            proxy.StringProperty = "Foo";

            Assert.Equal("Foo", proxy.StringProperty);
            Assert.True(interceptor.PreInvokeHit);
            Assert.True(interceptor.InvokeHit);
            Assert.True(interceptor.PostInvokeHit);
        }
    }
}
