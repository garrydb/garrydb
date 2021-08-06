namespace GarryDB.Builders.Positions
{
    public class MoveBuilder<TBuilder> : EnPassantBuilder<MoveBuilder<TBuilder>>
        where TBuilder : MoveBuilder<TBuilder>
    {
        protected MoveBuilder()
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
