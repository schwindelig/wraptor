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
        public void MethodWithMultipleValueTypeParameters_IsInvokable()
        {
            var generator = new Generator();
            var interceptor = new UnitTestInterceptor();
            var originalInstance = new SampleClass();
            var proxy = generator.Generate<ISampleClass>(originalInstance, interceptor);

            var result1 = proxy.GetInt1(42, 1337);
            var result2 = proxy.GetInt2(42, 1337);

            Assert.Equal(42, result1);
            Assert.Equal(1337, result2);
            Assert.True(interceptor.PreInvokeHit);
            Assert.True(interceptor.InvokeHit);
            Assert.True(interceptor.PostInvokeHit);
        }

        [Fact]
        public void MethodWithMultipleReferenceTypeParameters_IsInvokable()
        {
            var generator = new Generator();
            var interceptor = new UnitTestInterceptor();
            var originalInstance = new SampleClass();
            var proxy = generator.Generate<ISampleClass>(originalInstance, interceptor);

            var result1 = proxy.GetString1("foo", "bar");
            var result2 = proxy.GetString2("foo", "bar");

            Assert.Equal("foo", result1);
            Assert.Equal("bar", result2);
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
