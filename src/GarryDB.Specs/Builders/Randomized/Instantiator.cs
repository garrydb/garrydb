using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FluentAssertions;

using GarryDB.Specs.Builders.Extensions;

namespace GarryDB.Specs.Builders.Randomized
{
    public sealed class Instantiator
    {
        private readonly ISet<string> overridden;
        private readonly IDictionary<string, object> overrides;
        private readonly int recursionCount;
        private readonly Type type;

        public Instantiator(Type type, IDictionary<string, object> overrides, int recursionCount)
        {
            this.type = type;
            this.overrides = overrides;
            this.recursionCount = recursionCount;
            overridden = new HashSet<string>();
        }

        public object CreateInstance()
        {
            if (typeof(Exception).IsAssignableFrom(type))
            {
                return Activator.CreateInstance(type)!;
            }

            ConstructorInfo constructor =
                type
                    .GetConstructors()
                    .OrderByDescending(x => x.GetParameters().Length)
                    .First();

            object[] arguments = GetConstructorArguments(constructor).ToArray();

            object result = constructor.Invoke(arguments);

            PopulateProperties(result);

            overrides.Keys.Should().BeEmpty("overrides should not contain keys that can't be resolved to a property.");

            return result;
        }

        private IEnumerable<object> GetConstructorArguments(ConstructorInfo constructor)
        {
            ParameterInfo[] constructorParameters = constructor.GetParameters();

            foreach (ParameterInfo parameter in constructorParameters)
            {
                object value = CreateValue(parameter.Name!, parameter.ParameterType);
                yield return value;
            }
        }

        private void PopulateProperties(object result)
        {
            IDictionary<string, PropertyInfo> properties =
                type
                    .GetProperties()
                    .Where(x => x.CanWrite &&
                                !overridden.Contains(x.Name.ToLower()) &&
                                x.PropertyType.IsDefaultValue(x.GetValue(result)!))
                    .ToDictionary(x => x.Name.ToLowerInvariant(), x => x);

            foreach (KeyValuePair<string, PropertyInfo> property in properties)
            {
                object value = CreateValue(property.Key, property.Value.PropertyType);
                property.Value.SetValue(result, value);
            }
        }

        private object CreateValue(string name, Type typeToGenerate)
        {
            string key = name.ToLower();

            object value;
            if (overrides.ContainsKey(key))
            {
                value = overrides[key];
                overrides.Remove(key);
                overridden.Add(key);
            }
            else
            {
                value = GenerateRandomValue(typeToGenerate);
            }

            return value;
        }

        private object GenerateRandomValue(Type typeToGenerate)
        {
            Type builderType;

            Type unwrapped = typeToGenerate.Unwrap();
            if (typeToGenerate.IsArray)
            {
                Type elementType = typeToGenerate.GetElementType()!;

                builderType = typeof(RandomArrayBuilder<>).MakeGenericType(elementType!);
            }
            else if (typeToGenerate.IsGenericType && typeToGenerate.GetGenericTypeDefinition() == typeof(ISet<>))
            {
                builderType = typeof(RandomSetBuilder<>).MakeGenericType(unwrapped);
            }
            else if (typeToGenerate.IsGenericType && typeToGenerate.GetGenericTypeDefinition() == typeof(IList<>))
            {
                builderType = typeof(RandomListBuilder<>).MakeGenericType(unwrapped);
            }
            else
            {
                builderType = typeof(RandomObjectBuilder<>).MakeGenericType(unwrapped);
            }

            dynamic builder = Activator.CreateInstance(builderType)!;

            if (builder is IRecursionSupport recursionSupport)
            {
                recursionSupport.WithRecursionCount(recursionCount + 1);
            }

            return builder!.Build();
        }
    }
}
