namespace GarryDB.Builders.Positions
{
    public class EnPassantBuilder<TBuilder> : CastlingBuilder<EnPassantBuilder<TBuilder>>
        where TBuilder : EnPassantBuilder<TBuilder>
    {
        protected EnPassantBuilder()
        {
            enPassant = null;
        }

        public TBuilder WithEnPassantSquare(Square square)
        {
            enPassant = square;

            return (TBuilder)this;
        }
    }
}