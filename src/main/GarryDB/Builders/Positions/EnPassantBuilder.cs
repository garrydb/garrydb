namespace GarryDB.Builders.Positions
{
    public abstract class EnPassantBuilder<TBuilder> : CastlingBuilder<EnPassantBuilder<TBuilder>>
        where TBuilder : EnPassantBuilder<TBuilder>
    {
        protected EnPassantBuilder()
        {
            EnPassant = null;
        }

        public TBuilder WithEnPassantSquare(Square square)
        {
            EnPassant = square;

            return (TBuilder)this;
        }
    }
}
