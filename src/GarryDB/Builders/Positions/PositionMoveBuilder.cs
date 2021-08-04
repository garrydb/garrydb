namespace GarryDB.Builders.Positions
{
    public class PositionMoveBuilder<TBuilder> : PositionEnPassantBuilder<PositionMoveBuilder<TBuilder>>
        where TBuilder : PositionMoveBuilder<TBuilder>
    {
        protected PositionMoveBuilder()
        {
            halfMoveClock = 0;
            fullMoveNumber = 1;
        }
        
        public TBuilder WitHalfMoveClock(int ply)
        {
            halfMoveClock = ply;

            return (TBuilder)this;
        }

        public TBuilder WithFullMove(int move)
        {
            fullMoveNumber = move;

            return (TBuilder)this;
        }
    }
}
