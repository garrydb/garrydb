using System.Collections.Generic;

namespace GarryDB.Builders.Positions
{
    public abstract class PositionBuilder : Position.Builder
    {
        protected IDictionary<Square, Piece> pieces;
        protected IList<Piece> castlingPossibilities;
        protected Color activeColor;
        protected Square? enPassant;
        protected int halfMoveClock;
        protected int fullMoveNumber;

        public Position Build()
        {
            return new Position(this);
        }

        IDictionary<Square, Piece> Position.Builder.Pieces
        {
            get { return pieces; }
        }

        IList<Piece> Position.Builder.CastlingPossibilities
        {
            get { return castlingPossibilities; }
        }

        Color Position.Builder.ActiveColor
        {
            get { return activeColor; }
        }

        Square? Position.Builder.EnPassant
        {
            get { return enPassant; }
        }

        int Position.Builder.HalfMoveClock
        {
            get { return halfMoveClock; }
        }

        int Position.Builder.FullMoveNumber
        {
            get { return fullMoveNumber; }
        }
    }
}
