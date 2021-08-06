using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GarryDB.Specs.Builders.Extensions
{
    internal static class TypeExtensions
    {
        public static Type Unwrap(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return Nullable.GetUnderlyingType(type)!;
            }

            if (type.IsGenericType && type.IsAssignableToGenericType(typeof(IEnumerable<>)))
            {
                return type.GetGenericArguments().Single();
            }

            return type;
        }

        public static bool IsDefaultValue(this Type type, object value)
        {
            if (type.IsValueType)
            {
                return Equals(Activator.CreateInstance(type), value);
            }

            if (value is IEnumerable e)
            {
                return !e.OfType<object>().Any();
            }

            return value == null;
        }

        private static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            Type[] interfaceTypes = givenType.GetInterfaces();

            if (interfaceTypes.Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
            {
                return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            Type baseType = givenType.BaseType;

            return baseType != null && IsAssignableToGenericType(baseType, genericType);
        }
    }
}
