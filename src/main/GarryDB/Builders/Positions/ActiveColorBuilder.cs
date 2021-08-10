namespace GarryDB.Builders.Positions
{
    public abstract class ActiveColorBuilder<TBuilder> : PieceBuilder<ActiveColorBuilder<TBuilder>>
        where TBuilder : ActiveColorBuilder<TBuilder>
    {
        protected ActiveColorBuilder()
        {
            ActiveColor = Color.White;
        }

        public TBuilder WithActiveColor(Color activeColor)
        {
            ActiveColor = activeColor;

            return (TBuilder)this;
        }
    }
}
