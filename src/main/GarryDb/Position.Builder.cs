using System.Collections.Generic;

using GarryDb.Builders.Positions;

namespace GarryDb
{
    public sealed partial class Position
    {
        public static BuilderDirector New()
        {
            return new BuilderDirector();
        }

        internal interface Builder
        {
            IDictionary<Square, Piece> Pieces { get; }
            IEnumerable<Piece> CastlingPossibilities { get; }
            Color ActiveColor { get; }
            Square? EnPassant { get; }
            int HalfMoveClock { get; }
            int FullMoveNumber { get; }
        }
    }
}
