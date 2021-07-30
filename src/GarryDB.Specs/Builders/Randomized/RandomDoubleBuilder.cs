using System;

namespace GarryDB.Specs.Builders.Randomized
{
    public sealed class RandomDoubleBuilder : TestDataBuilder<double>
    {
        private double maximum = 32767;
        private double minimum;

        protected override double OnBuild()
        {
            var random = new Random();
            double randomDouble = random.NextDouble() * Convert.ToDouble(maximum - minimum) + Convert.ToDouble(minimum);

            return Convert.ToDouble(randomDouble);
        }

        public RandomDoubleBuilder WithMinimum(double newMinimum)
        {
            minimum = newMinimum;
            return this;
        }

        public RandomDoubleBuilder WithMaximum(double newMaximum)
        {
            maximum = newMaximum;
            return this;
        }
    }
}
