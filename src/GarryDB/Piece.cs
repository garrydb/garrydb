using System;

namespace GarryDB
{
    public abstract class Piece
    {
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
