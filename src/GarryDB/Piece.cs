using System;
using System.Linq;

namespace GarryDB
{
    public abstract class Piece
    {
        public static class White
        {
            public static readonly Piece King = new King(Color.White);
            public static readonly Piece Queen = new Queen(Color.White);
            public static readonly Piece Rook = new Rook(Color.White);
            public static readonly Piece Bishop = new Bishop(Color.White);
            public static readonly Piece Knight = new Knight(Color.White);
            public static readonly Piece Pawn = new Pawn(Color.White);
        }

        public static class Black
        {
            public static readonly Piece King = new King(Color.Black);
            public static readonly Piece Queen = new Queen(Color.Black);
            public static readonly Piece Rook = new Rook(Color.Black);
            public static readonly Piece Bishop = new Bishop(Color.Black);
            public static readonly Piece Knight = new Knight(Color.Black);
            public static readonly Piece Pawn = new Pawn(Color.Black);
        }

        public static readonly Piece None = new None();

        public static Piece Parse(string symbol, Color color)
        {
            if (color == Color.White)
            {
                return new[]
                       {
                           White.King,
                           White.Queen,
                           White.Rook,
                           White.Bishop,
                           White.Knight,
                           White.Pawn
                       }.SingleOrDefault(piece => piece.symbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase)) ?? None;
            }

            return new[]
                   {
                       Black.King,
                       Black.Queen,
                       Black.Rook,
                       Black.Bishop,
                       Black.Knight,
                       Black.Pawn
                   }.SingleOrDefault(piece => piece.symbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase)) ?? None;
        }

        private readonly string symbol;
        private readonly Color color;

        protected Piece(string symbol, Color color)
        {
            this.symbol = symbol;
            this.color = color;
        }

        public override bool Equals(object? obj)
        {
            return obj is Piece that && symbol == that.symbol&& color == that.color;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(symbol, color);
        }

        public override string ToString()
        {
            return color == Color.White ? symbol.ToUpperInvariant() : symbol.ToLowerInvariant();
        }

        public static bool operator ==(Piece? p1, Piece? p2)
        {
            if (ReferenceEquals(p1, null))
            {
                return false;
            }

            if (ReferenceEquals(p2, null))
            {
                return false;
            }

            return p1.Equals(p2);
        }

        public static bool operator !=(Piece? p1, Piece? p2)
        {
            return !(p1 == p2);
        }
    }
}
