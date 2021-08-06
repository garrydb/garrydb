namespace GarryDB.Specs.Extensions
{
    internal static class PositionExtensions
    {
        public static PositionAssertions Should(this Position position)
        {
            return new PositionAssertions(position);
        }
    }
}
