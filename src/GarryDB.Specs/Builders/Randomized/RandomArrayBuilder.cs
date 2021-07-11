using System;
using System.Linq;

namespace GarryDb.Specs.Builders.Randomized
{
    public sealed class RandomArrayBuilder<T> : TestDataBuilder<T[]>, IRecursionSupport
    {
        private int maximum = 5;
        private int minimum = 1;
        private int recursionCount;
        private Func<object> values;

        public void WithRecursionCount(int recursionCount)
        {
            this.recursionCount = recursionCount;
        }

        protected override void OnPreBuild()
        {
            if (values == null)
            {
                With(new object());
            }
        }

        protected override T[] OnBuild()
        {
            int arrayLength = new RandomIntegerBuilder().WithMinimum(minimum).WithMaximum(maximum).Build();
            int maxRecursionCount = new RandomIntegerBuilder().WithMinimum(0).WithMaximum(3 - recursionCount).Build();

            if (arrayLength == 0 || recursionCount > maxRecursionCount)
            {
                return Array.Empty<T>();
            }

            return
                Enumerable.Range(1, arrayLength)
                          .Select(_ => new RandomObjectBuilder<T>().WithRecursionCount(recursionCount).With(values!).Build())
                          .ToArray();
        }

        public RandomArrayBuilder<T> WithSize(int size)
        {
            return WithMinimum(size).WithMaximum(size);
        }

        public RandomArrayBuilder<T> WithMinimum(int newMinimum)
        {
            minimum = newMinimum;

            return WithMaximum(Math.Max(maximum, minimum));
        }

        public RandomArrayBuilder<T> WithMaximum(int newMaximum)
        {
            maximum = newMaximum;
            return this;
        }

        public RandomArrayBuilder<T> With(object presetValues)
        {
            return With(() => presetValues);
        }

        public RandomArrayBuilder<T> With(Func<object> presetValues)
        {
            values = presetValues;
            return this;
        }
    }
}
