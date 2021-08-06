namespace GarryDB.Specs.Extensions
{
    public static class PositionExtensions
    {
        public static PositionAssertions Should(this Position position)
        {
            return new PositionAssertions(position);
        }
    }
}
