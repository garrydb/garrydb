using System.Collections.Generic;

using GarryDB.Builders.Positions;

namespace GarryDB
{
    public sealed partial class Position
    {
        public static PositionBuilderDirector New()
        {
            return new();
        }

        internal interface Builder
        {
            IDictionary<Square, Piece> Pieces { get; }
            IList<Piece> CastlingPossibilities { get; }
            Color ActiveColor { get; }
            Square? EnPassant { get; }
            int HalfMoveClock { get; }
            int FullMoveNumber { get; }
        }
    }
}
