using System;

namespace GarryDB.Specs.Builders.Randomized
{
    internal sealed class RandomDecimalBuilder : TestDataBuilder<decimal>
    {
        private decimal maximum = 32767;
        private decimal minimum;

        protected override decimal OnBuild()
        {
            var random = new Random();
            double randomDouble = random.NextDouble() * Convert.ToDouble(maximum - minimum) + Convert.ToDouble(minimum);

            return Convert.ToDecimal(randomDouble);
        }

        public RandomDecimalBuilder WithMinimum(decimal newMinimum)
        {
            minimum = newMinimum;

            return this;
        }

        public RandomDecimalBuilder WithMaximum(decimal newMaximum)
        {
            maximum = newMaximum;

            return this;
        }
    }
}
