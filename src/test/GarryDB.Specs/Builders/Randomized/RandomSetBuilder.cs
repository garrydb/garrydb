using System;
using System.Collections.Generic;
using System.Linq;

namespace GarryDB.Specs.Builders.Randomized
{
    internal sealed class RandomSetBuilder<T> : TestDataBuilder<ISet<T>>, IRecursionSupport
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

        protected override ISet<T> OnBuild()
        {
            int size = new RandomIntegerBuilder().WithMinimum(minimum).WithMaximum(maximum).Build();
            int maxRecursionCount = new RandomIntegerBuilder().WithMinimum(0).WithMaximum(3 - recursionCount).Build();

            if (size == 0 || recursionCount > maxRecursionCount)
            {
                return new HashSet<T>();
            }

            return Enumerable.Range(1, size)
                             .Select(x => new RandomObjectBuilder<T>().WithRecursionCount(recursionCount).With(values!).Build())
                             .ToHashSet()!;
        }

        public RandomSetBuilder<T> WithSize(int size)
        {
            return WithMinimum(size).WithMaximum(size);
        }

        public RandomSetBuilder<T> WithMinimum(int newMinimum)
        {
            minimum = newMinimum;

            return WithMaximum(Math.Max(maximum, minimum));
        }

        public RandomSetBuilder<T> WithMaximum(int newMaximum)
        {
            maximum = newMaximum;

            return this;
        }

        public RandomSetBuilder<T> With(object presetValues)
        {
            return With(() => presetValues);
        }

        public RandomSetBuilder<T> With(Func<object> presetValues)
        {
            values = presetValues;

            return this;
        }
    }
}
