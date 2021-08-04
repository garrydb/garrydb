namespace GarryDB.Builders.Positions
{
    public class PositionEnPassantBuilder<TBuilder> : PositionCastlingBuilder<PositionEnPassantBuilder<TBuilder>>
        where TBuilder : PositionEnPassantBuilder<TBuilder>
    {
        protected PositionEnPassantBuilder()
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
