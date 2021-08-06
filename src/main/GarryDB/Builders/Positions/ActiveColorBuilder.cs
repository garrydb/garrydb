namespace GarryDB.Builders.Positions
{
    public class ActiveColorBuilder<TBuilder> : PieceBuilder<ActiveColorBuilder<TBuilder>>
        where TBuilder : ActiveColorBuilder<TBuilder>
    {
        protected ActiveColorBuilder()
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
