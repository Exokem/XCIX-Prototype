
using System;
using System.Runtime.CompilerServices;
using System.Reflection;
using System.Collections.Generic;
using Xylem.Data;

namespace Xylem.Reflection
{
    public static class Introspector
    {
        public static V Instantiate<V>(string scopedClassName, Type requiredBaseType, params object[] constructionArguments) => Instantiate<V>(Type.GetType(scopedClassName), requiredBaseType, constructionArguments);

        public static V Instantiate<V>(Type instanceType, Type requiredBaseType, params object[] constructionArguments)
        {
            Type nextBaseType = instanceType.BaseType;

            if (instanceType != requiredBaseType)
            {
                while (nextBaseType != requiredBaseType && nextBaseType != null)
                {
                    nextBaseType = nextBaseType.BaseType;
                }

                if (nextBaseType == null)
                    throw new InvalidCastException($"Cannot reduce actual instance base type '{nextBaseType}' to required base type '{requiredBaseType}'");
            }

            V instance = (V) Activator.CreateInstance(instanceType, constructionArguments);

            return instance;
        }
    }

    public static class Initializer
    {
        private static readonly Assembly _assembly;

        static Initializer()
        {
            _assembly = Assembly.GetExecutingAssembly();
        }

        public static void Initialize<V>(string initializationType) where V : ModuleAttribute
        {
            Dictionary<string, List<Type>> initializers = new Dictionary<string, List<Type>>();

            foreach (Type type in _assembly.GetTypes())
            {
                V initializer = type.GetCustomAttribute<V>();

                if (initializer != null)
                {
                    List<Type> types = initializers.GetValueOrDefault(initializer.Identifier, new List<Type>());
                    types.Add(type);
                    initializers[initializer.Identifier] = types;
                }
            }

            foreach (string moduleIdentifier in Importer.IndexedModuleOrder)
            {
                if (initializers.ContainsKey(moduleIdentifier))
                {
                    List<Type> types = initializers[moduleIdentifier];

                    foreach (Type type in types)
                        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                    
                    Output.Write($"Initialized {types.Count} '{initializationType}'", moduleIdentifier);
                }
            }
        }

        public static void Initialize(Type attributeType)
        {
            foreach (Type type in _assembly.GetTypes())
            {
                if (type.GetCustomAttributes(attributeType, true).Length > 0)
                {
                    RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                }
            }
        }
    }
}