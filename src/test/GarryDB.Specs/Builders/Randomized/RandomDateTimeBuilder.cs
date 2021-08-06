using System;

namespace GarryDB.Specs.Builders.Randomized
{
    internal sealed class RandomDateTimeBuilder : TestDataBuilder<DateTime>
    {
        private DateTime baseMoment = DateTime.Now;
        private bool includeTimeComponent = true;
        private TimeSpan maximum = TimeSpan.FromTicks(int.MaxValue);
        private TimeSpan minimum = TimeSpan.FromTicks(int.MinValue);

        public RandomDateTimeBuilder ThatIsInThePast
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

        public RandomDateTimeBuilder ThatIsInTheFuture
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

        public RandomDateTimeBuilder WithoutTimeComponent
        {
            get
            {
                includeTimeComponent = false;

                return this;
            }
        }

        protected override DateTime OnBuild()
        {
            int minimumTicks = (int)minimum.Ticks;
            int maximumTicks = (int)maximum.Ticks;

            int randomTicks = new RandomIntegerBuilder().WithMinimum(minimumTicks).WithMaximum(maximumTicks).Build();

            var period = TimeSpan.FromTicks(randomTicks);

            DateTime dateTime = baseMoment.Add(period);

            if (includeTimeComponent)
            {
                return dateTime;
            }

            return dateTime.Date;
        }

        public RandomDateTimeBuilder ThatIsAfter(DateTime afterMoment)
        {
            baseMoment = afterMoment;
            minimum = TimeSpan.Zero;

            return this;
        }
    }
}
