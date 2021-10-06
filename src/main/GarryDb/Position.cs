using System.Collections.Generic;

namespace GarryDb
{
    public sealed partial class Position
    {
        private readonly IDictionary<Square, Piece> pieces;
        private readonly IList<Piece> castlingPossibilities;
        private readonly Color activeColor;
        private readonly Square? enPassant;
        private readonly int halfMoveClock;
        private readonly int fullMoveNumber;

        internal Position(Builder builder)
        {
            pieces = new Dictionary<Square, Piece>(builder.Pieces);
            castlingPossibilities = new List<Piece>(builder.CastlingPossibilities);
            activeColor = builder.ActiveColor;
            enPassant = builder.EnPassant;
            halfMoveClock = builder.HalfMoveClock;
            fullMoveNumber = builder.FullMoveNumber;
        }
    }
}
