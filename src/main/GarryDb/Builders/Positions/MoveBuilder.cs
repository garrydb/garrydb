namespace GarryDb.Builders.Positions
{
    public abstract class MoveBuilder<TBuilder> : EnPassantBuilder<MoveBuilder<TBuilder>>
        where TBuilder : MoveBuilder<TBuilder>
    {
        protected MoveBuilder()
        {
            HalfMoveClock = 0;
            FullMoveNumber = 1;
        }
        
        public TBuilder WitHalfMoveClock(int ply)
        {
            HalfMoveClock = ply;

            return (TBuilder)this;
        }

        public TBuilder WithFullMove(int move)
        {
            FullMoveNumber = move;

            return (TBuilder)this;
        }
    }
}
