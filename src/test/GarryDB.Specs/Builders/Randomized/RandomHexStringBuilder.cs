namespace GarryDB.Specs.Builders.Randomized
{
    internal sealed class RandomHexStringBuilder : TestDataBuilder<string>
    {
        private long value;

        public RandomHexStringBuilder()
        {
            value = new RandomIntegerBuilder().Build();
        }

        protected override string OnBuild()
        {
            return value.ToString("X");
        }

        public RandomHexStringBuilder ForValue(long value)
        {
            this.value = value;

            return this;
        }
    }
}
