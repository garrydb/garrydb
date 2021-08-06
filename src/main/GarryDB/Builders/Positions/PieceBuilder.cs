using System.Collections.Generic;
using System.Linq;

using GarryDB.Pieces;

namespace GarryDB.Builders.Positions
{
    public class PieceBuilder<TBuilder> : PositionBuilder where TBuilder : PieceBuilder<TBuilder>
    {
        protected PieceBuilder()
        {
            pieces = new Dictionary<Square, Piece>();

            foreach (string file in Enumerable.Range('a', 8).Select(i => char.ToString((char)i)))
            {
                foreach (int rank in Enumerable.Range(1, 8))
                {
                    pieces[new Square(file, rank)] = new None();
                }
            }
        }

        public TBuilder WithPieces(IDictionary<Square, Piece> pieces)
        {
            foreach ((Square square, Piece piece) in pieces)
            {
                this.pieces[square] = piece;
            }

            return (TBuilder)this;
        }

        public TBuilder WithPiece(Square square, Piece piece)
        {
            pieces[square] = piece;

            return (TBuilder)this;
        }
    }
}
