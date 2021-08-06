using System;

namespace GarryDB.Specs.Builders.Randomized
{
    internal sealed class RandomIntegerBuilder : TestDataBuilder<int>
    {
        private int maximum = 32767;
        private int minimum = -32767;

        protected override int OnBuild()
        {
            var random = new Random();

            return random.Next(minimum, maximum);
        }

        public RandomIntegerBuilder WithMinimum(int newMinimum)
        {
            minimum = newMinimum;

            return this;
        }

        public RandomIntegerBuilder WithMaximum(int newMaximum)
        {
            maximum = newMaximum;

            return this;
        }
    }
}
