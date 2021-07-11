namespace GarryDb.Specs.Builders.Randomized
{
    public sealed class RandomBooleanBuilder : TestDataBuilder<bool>
    {
        protected override bool OnBuild()
        {
            int value = new RandomIntegerBuilder().WithMinimum(0).WithMaximum(10).Build();
            return value < 5;
        }
    }
}
