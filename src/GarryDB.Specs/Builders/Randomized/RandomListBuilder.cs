using System;
using System.Collections.Generic;
using System.Linq;

namespace GarryDB.Specs.Builders.Randomized
{
    public sealed class RandomListBuilder<T> : TestDataBuilder<IList<T>>, IRecursionSupport
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

        protected override IList<T> OnBuild()
        {
            int size = new RandomIntegerBuilder().WithMinimum(minimum).WithMaximum(maximum).Build();
            int maxRecursionCount = new RandomIntegerBuilder().WithMinimum(0).WithMaximum(3 - recursionCount).Build();

            if (size == 0 || recursionCount > maxRecursionCount)
            {
                return new List<T>();
            }

            return
                Enumerable.Range(1, size)
                          .Select(_ => new RandomObjectBuilder<T>().WithRecursionCount(recursionCount).With(values!).Build())
                          .ToList()!;
        }

        public RandomListBuilder<T> WithSize(int size)
        {
            return WithMinimum(size).WithMaximum(size);
        }

        public RandomListBuilder<T> WithMinimum(int newMinimum)
        {
            minimum = newMinimum;

            return WithMaximum(Math.Max(maximum, minimum));
        }

        public RandomListBuilder<T> WithMaximum(int newMaximum)
        {
            maximum = newMaximum;
            return this;
        }

        public RandomListBuilder<T> With(object preListValues)
        {
            return With(() => preListValues);
        }

        public RandomListBuilder<T> With(Func<object> preListValues)
        {
            values = preListValues;
            return this;
        }
    }
}
