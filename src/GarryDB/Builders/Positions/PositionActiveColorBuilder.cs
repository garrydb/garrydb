namespace GarryDB.Builders.Positions
{
    public class PositionActiveColorBuilder<TBuilder> : PositionPieceBuilder<PositionActiveColorBuilder<TBuilder>>
        where TBuilder : PositionActiveColorBuilder<TBuilder>
    {
        protected PositionActiveColorBuilder()
        {
            activeColor = Color.White;
        }

        public TBuilder WithActiveColor(Color activeColor)
        {
            this.activeColor = activeColor;

            return (TBuilder)this;
        }
    }
}
