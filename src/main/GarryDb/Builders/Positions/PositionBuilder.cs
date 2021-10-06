using System.Collections.Generic;

namespace GarryDb.Builders.Positions
{
    public abstract class PositionBuilder : Position.Builder
    {
        protected IDictionary<Square, Piece> Pieces = null!;
        protected IList<Piece> CastlingPossibilities = null!;
        protected Color ActiveColor = null!;
        protected Square? EnPassant;
        protected int HalfMoveClock;
        protected int FullMoveNumber;

        public Position Build()
        {
            return new Position(this);
        }

        IDictionary<Square, Piece> Position.Builder.Pieces
        {
            get { return Pieces; }
        }

        IEnumerable<Piece> Position.Builder.CastlingPossibilities
        {
            get { return CastlingPossibilities; }
        }

        Color Position.Builder.ActiveColor
        {
            get { return ActiveColor; }
        }

        Square? Position.Builder.EnPassant
        {
            get { return EnPassant; }
        }

        int Position.Builder.HalfMoveClock
        {
            get { return HalfMoveClock; }
        }

        int Position.Builder.FullMoveNumber
        {
            get { return FullMoveNumber; }
        }
    }
}
