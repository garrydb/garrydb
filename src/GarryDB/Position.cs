using System.Collections.Generic;
using System.Linq;

namespace GarryDB
{
    public sealed class Position
    {
        public static readonly Position Empty;

        static Position()
        {
            var pieces = new Dictionary<Square, Piece>();
            foreach (string file in Enumerable.Range('a', 8).Select(i => i.ToString()))
            {
                foreach (int rank in Enumerable.Range(0, 7))
                {
                    pieces[new Square(file, rank + 1)] = Piece.None;
                }
            }

            Empty = new Position(pieces);
        }

        private readonly IDictionary<Square, Piece> pieces;

        public Position(IDictionary<Square, Piece> pieces)
        {
            this.pieces = pieces;
        }

        public Piece this[Square square]
        {
            get { return pieces[square]; }
            set { pieces[square] = value; }
        }
    }
}