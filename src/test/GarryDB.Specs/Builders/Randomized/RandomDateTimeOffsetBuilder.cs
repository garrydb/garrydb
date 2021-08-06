using System;

namespace GarryDB.Specs.Builders.Randomized
{
    public sealed class RandomDateTimeOffsetBuilder : TestDataBuilder<DateTimeOffset>
    {
        private bool includeTimeComponent = true;
        private TimeSpan maximum = TimeSpan.FromTicks(int.MaxValue);
        private TimeSpan minimum = TimeSpan.FromTicks(int.MinValue);

        public RandomDateTimeOffsetBuilder ThatIsInThePast
        {
            get
            {
                maximum = TimeSpan.Zero;

                if (minimum >= maximum)
                {
                    minimum = TimeSpan.MinValue;
                }

                return this;
            }
        }

        public RandomDateTimeOffsetBuilder ThatIsInTheFuture
        {
            get
            {
                minimum = TimeSpan.Zero;

                if (maximum <= minimum)
                {
                    maximum = TimeSpan.MaxValue;
                }

                return this;
            }
        }

        public RandomDateTimeOffsetBuilder WithoutTimeComponent
        {
            get
            {
                includeTimeComponent = false;

                return this;
            }
        }

        protected override DateTimeOffset OnBuild()
        {
            int minimumTicks = (int)minimum.Ticks;
            int maximumTicks = (int)maximum.Ticks;

            int randomTicks = new RandomIntegerBuilder().WithMinimum(minimumTicks).WithMaximum(maximumTicks).Build();

            var period = TimeSpan.FromTicks(randomTicks);

            DateTimeOffset dateTimeOffset = DateTimeOffset.Now.Add(period);

            if (includeTimeComponent)
            {
                return new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour,
                                          dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Offset);
            }

            return dateTimeOffset.Date;
        }
    }
}
