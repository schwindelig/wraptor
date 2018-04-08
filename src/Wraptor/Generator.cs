using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Wraptor
{
    public class Generator
    {
        private struct MethodGenerationDefinition
        {
            public struct LocalIndexes
            {
                public const int TypeIdentifier = 0;
                public const int MethodInfo = 1;
                public const int Arguments = 2;
                public const int ReturnValue = 3;
            }
        }

        public TWrapped Generate<TWrapped>(
            TWrapped implementation,
            IInterceptor interceptor
            ) where TWrapped : class
        {
            // TODO: Add check if TWrapped is an interface

            var wrappedType = typeof(TWrapped);
            if (!wrappedType.IsInterface)
            {
                throw new ArgumentException($"{nameof(TWrapped)} must be an interface");
            }

            var typeBuilder = this.CreateTypeBuilder<TWrapped>();
            var typeInfo = typeBuilder.CreateTypeInfo();
            var typeInfoAsType = typeInfo.AsType();
            var instance = Activator.CreateInstance(
                typeInfoAsType,
                implementation,
                interceptor
                ) as TWrapped;

            return instance;
        }

        protected virtual TypeBuilder CreateTypeBuilder<TInterface>() where TInterface : class
        {
            var interfaceType = typeof(TInterface);
            var interceptorType = typeof(IInterceptor);

            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()),
                AssemblyBuilderAccess.Run);

            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");

            var typeBuilder = moduleBuilder.DefineType("Wraptor.Generated",
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                null);

            // Define the field holding the implementing class
            var implementationFieldBuilder =
                typeBuilder.DefineField("_implementation", interfaceType, FieldAttributes.Private);

            // Define the field holding the interceptor
            var interceptorFieldBuilder =
                typeBuilder.DefineField("_interceptor", interceptorType, FieldAttributes.Private);

            // Define the constructor
            this.ApplyConstructor(typeBuilder, implementationFieldBuilder, interceptorFieldBuilder, interfaceType, interceptorType);

            // Inherit from defined interface
            typeBuilder.AddInterfaceImplementation(interfaceType);

            // Apply Method Implementations
            this.ApplyMethodImplementations(typeBuilder, implementationFieldBuilder, interceptorFieldBuilder, interfaceType, interceptorType);

            return typeBuilder;
        }

        protected virtual void ApplyConstructor(
            TypeBuilder typeBuilder,
            FieldBuilder implementationFieldBuilder,
            FieldBuilder interceptorFieldBuilder,
            Type interfaceType,
            Type interceptorType)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(
                            MethodAttributes.Public |
                            MethodAttributes.SpecialName |
                            MethodAttributes.RTSpecialName,
                            CallingConventions.Standard,
                            new[]
                            {
                                interfaceType,
                                interceptorType
                            });

            var ilGenerator = constructorBuilder.GetILGenerator();

            // Load "this"
            ilGenerator.Emit(OpCodes.Ldarg_0);

            // Load "implementation" parameter
            ilGenerator.Emit(OpCodes.Ldarg_1);

            // Set "_implementation" field
            ilGenerator.Emit(OpCodes.Stfld, implementationFieldBuilder);

            // Load "this"
            ilGenerator.Emit(OpCodes.Ldarg_0);

            // Load "interceptor" parameter
            ilGenerator.Emit(OpCodes.Ldarg_2);

            // Set "_interceptor" field
            ilGenerator.Emit(OpCodes.Stfld, interceptorFieldBuilder);

            // Return
            ilGenerator.Emit(OpCodes.Ret);
        }

        protected virtual void ApplyMethodImplementations(
            TypeBuilder typeBuilder,
            FieldBuilder implementationFieldBuilder,
            FieldBuilder interceptorFieldBuilder,
            Type interfaceType,
            Type interceptorType)
        {
            // TODO: Add check if Type is interface
            var typeIdentifier = MetaInfoProvider.GetTypeIdentifier(interfaceType);
            var methodInfos = MetaInfoProvider.GetMethodInfos(interfaceType).ToArray();
            var getMethodInfoMethodInfo = typeof(MetaInfoProvider).GetMethod(
                nameof(MetaInfoProvider.GetMethodInfo));
            var preInvokeMethodInfo = typeof(IInterceptor).GetMethod(nameof(IInterceptor.PreInvoke));
            var postInvokeMethodInfo = typeof(IInterceptor).GetMethod(nameof(IInterceptor.PostInvoke));
            var invokeMethodInfo = typeof(IInterceptor).GetMethod(nameof(IInterceptor.Invoke));
            var objectType = typeof(object);

            for (var methodIndex = 0; methodIndex < methodInfos.Length; methodIndex++)
            {
                var methodInfo = methodInfos.ElementAt(methodIndex);
                var parameterInfos = MetaInfoProvider.GetParameterInfos(methodInfo);
                var returnType = MetaInfoProvider.GetReturnType(methodInfo);

                var methodBuilder = typeBuilder.DefineMethod(
                    methodInfo.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    CallingConventions.Standard,
                    methodInfo.ReturnType,
                    methodInfo.GetParameters().Select(p => p.ParameterType).ToArray());

                var ilGenerator = methodBuilder.GetILGenerator();
                // Locals:
                // 0: string: Stores the typeIdentifier
                // 1: MethoodInfo: Stores the MethodInfo of the currently wrapped method
                // 2: object[]: Stores the arguments passed to the method
                // 3: object: Stores the boxed result returned from IInterceptor.Invoke
                ilGenerator.DeclareLocal(typeof(string)); // typeIdentifier
                ilGenerator.DeclareLocal(typeof(MethodInfo)); // methodInfo
                ilGenerator.DeclareLocal(typeof(object[])); // arguments
                ilGenerator.DeclareLocal(typeof(object)); // returnValue

                // Load typeIdentifier string and store in loc_0
                ilGenerator.Emit(OpCodes.Ldstr, typeIdentifier);
                ilGenerator.Emit(OpCodes.Stloc, MethodGenerationDefinition.LocalIndexes.TypeIdentifier);

                // Call GetMethodInfo on MetaInfoProvider and store in loc_1
                ilGenerator.Emit(OpCodes.Ldloc, MethodGenerationDefinition.LocalIndexes.TypeIdentifier);
                ilGenerator.Emit(OpCodes.Ldc_I4, methodIndex);
                ilGenerator.Emit(OpCodes.Call, getMethodInfoMethodInfo);
                ilGenerator.Emit(OpCodes.Stloc, MethodGenerationDefinition.LocalIndexes.MethodInfo);

                // Create an object array to hold the parameters. Size is the amount of parameters the original method has defined
                ilGenerator.Emit(OpCodes.Ldc_I4, parameterInfos.ParameterCount);
                ilGenerator.Emit(OpCodes.Newarr, objectType);
                ilGenerator.Emit(OpCodes.Stloc, MethodGenerationDefinition.LocalIndexes.Arguments);

                // Populate the argument array
                for (var parameterIndex = 0; parameterIndex < parameterInfos.ParameterCount; parameterIndex++)
                {
                    ilGenerator.Emit(OpCodes.Ldloc, MethodGenerationDefinition.LocalIndexes.Arguments); // arguments
                    ilGenerator.Emit(OpCodes.Ldc_I4, parameterIndex); // array index
                    ilGenerator.Emit(OpCodes.Ldarg, parameterIndex + 1); // need to add 1 here, as arg 0 is "this"
                    if (parameterInfos.IsParameterValueType(parameterIndex))
                    {
                        ilGenerator.Emit(OpCodes.Box, parameterInfos.GetParameterType(parameterIndex));
                    }
                    ilGenerator.Emit(OpCodes.Stelem_Ref);
                }

                // Call PreInvoke method on _interceptor field
                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, interceptorFieldBuilder);
                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, implementationFieldBuilder);
                ilGenerator.Emit(OpCodes.Ldloc, MethodGenerationDefinition.LocalIndexes.MethodInfo); // methodInfo
                ilGenerator.Emit(OpCodes.Ldloc, MethodGenerationDefinition.LocalIndexes.Arguments); // arguments
                ilGenerator.Emit(OpCodes.Callvirt, preInvokeMethodInfo);

                // Call Invoke method on _interceptor field
                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, interceptorFieldBuilder);
                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, implementationFieldBuilder);
                ilGenerator.Emit(OpCodes.Ldloc, MethodGenerationDefinition.LocalIndexes.MethodInfo); // methodInfo
                ilGenerator.Emit(OpCodes.Ldloc, MethodGenerationDefinition.LocalIndexes.Arguments); // arguments
                ilGenerator.Emit(OpCodes.Callvirt, invokeMethodInfo);

                // Handle return type
                if (returnType == typeof(void))
                {
                    // If the return type is void, we have to pop it, as we otherwise would return garbage
                    ilGenerator.Emit(OpCodes.Pop);
                }
                else
                {
                    // Store the boxed return value in loc 3
                    ilGenerator.Emit(OpCodes.Stloc, MethodGenerationDefinition.LocalIndexes.ReturnValue);
                }

                // Call PostInvoke method on _interceptor field
                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, interceptorFieldBuilder);
                ilGenerator.Emit(OpCodes.Ldarg_0); // this
                ilGenerator.Emit(OpCodes.Ldfld, implementationFieldBuilder);
                ilGenerator.Emit(OpCodes.Ldloc, MethodGenerationDefinition.LocalIndexes.MethodInfo); // methodInfo
                ilGenerator.Emit(OpCodes.Ldloc, MethodGenerationDefinition.LocalIndexes.Arguments); // arguments
                ilGenerator.Emit(OpCodes.Ldloc, MethodGenerationDefinition.LocalIndexes.ReturnValue); // returnValue
                ilGenerator.Emit(OpCodes.Callvirt, postInvokeMethodInfo);

                // Prepare return value (if the original method isn't void)
                if (returnType != typeof(void))
                {
                    // Load the boxed result onto the evaluation stack
                    ilGenerator.Emit(OpCodes.Ldloc, MethodGenerationDefinition.LocalIndexes.ReturnValue);

                    if (returnType.IsValueType)
                    {
                        // Unbox value types
                        ilGenerator.Emit(OpCodes.Unbox_Any, returnType);
                    }
                }

                ilGenerator.Emit(OpCodes.Ret);
            }
        }
    }
}
