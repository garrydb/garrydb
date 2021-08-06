using System;
using System.Collections.Generic;
using System.Linq;

namespace GarryDB.Specs.Builders.Randomized
{
    internal sealed class RandomObjectBuilder<T> : TestDataBuilder<T>
    {
        private readonly IDictionary<Type, Func<object>> randomGenerators = new Dictionary<Type, Func<object>>
                                                                            {
                                                                                {
                                                                                    typeof(Guid), () => Guid.NewGuid()
                                                                                },
                                                                                {
                                                                                    typeof(string),
                                                                                    () => new RandomStringBuilder().Build()
                                                                                },
                                                                                {
                                                                                    typeof(DateTimeOffset),
                                                                                    () => new RandomDateTimeOffsetBuilder()
                                                                                        .Build()
                                                                                },
                                                                                {
                                                                                    typeof(DateTime),
                                                                                    () => new RandomDateTimeBuilder().Build()
                                                                                },
                                                                                {
                                                                                    typeof(decimal),
                                                                                    () => new RandomDecimalBuilder().Build()
                                                                                },
                                                                                {
                                                                                    typeof(double),
                                                                                    () => new RandomDoubleBuilder().Build()
                                                                                },
                                                                                {
                                                                                    typeof(byte),
                                                                                    () => (byte)new RandomIntegerBuilder()
                                                                                        .WithMinimum(byte.MinValue)
                                                                                        .WithMaximum(byte.MaxValue)
                                                                                        .Build()
                                                                                },
                                                                                {
                                                                                    typeof(int),
                                                                                    () => new RandomIntegerBuilder().Build()
                                                                                },
                                                                                {
                                                                                    typeof(long),
                                                                                    () => (long)new RandomIntegerBuilder().Build()
                                                                                },
                                                                                {
                                                                                    typeof(bool),
                                                                                    () => new RandomBooleanBuilder().Build()
                                                                                }
                                                                            };

        private int recursionCount;

        private Func<object> valuesGenerator;

        protected override void OnPreBuild()
        {
            if (valuesGenerator == null)
            {
                With(() => new object());
            }
        }

        protected override T OnBuild()
        {
            Type type = DetermineType();

            if (randomGenerators.ContainsKey(type))
            {
                Func<object> generator = randomGenerators[type];

                return (T)generator();
            }

            object values = valuesGenerator!();
            IDictionary<string, object> overrides = values.GetType()
                                                          .GetProperties()
                                                          .ToDictionary(x => x.Name.ToLowerInvariant(), x => x.GetValue(values))!;

            var instantiator = new Instantiator(type, overrides, recursionCount);

            var result = (T)instantiator.CreateInstance();

            return result;
        }

        public RandomObjectBuilder<T> WithRecursionCount(int recursionCount)
        {
            this.recursionCount = recursionCount;

            return this;
        }

        public RandomObjectBuilder<T> With(object values)
        {
            return With(() => values);
        }

        public RandomObjectBuilder<T> With(Func<object> values)
        {
            valuesGenerator = values;

            return this;
        }

        private Type DetermineType()
        {
            Type type = typeof(T);
            if (!type.IsAbstract && !type.IsInterface)
            {
                return type;
            }

            Type subtype = type.Assembly.GetTypes()
                               .Where(x => type.IsAssignableFrom(x) && !x.IsAbstract && !x.IsInterface)
                               .OrderBy(_ => Guid.NewGuid())
                               .First();

            return subtype;
        }
    }
}
