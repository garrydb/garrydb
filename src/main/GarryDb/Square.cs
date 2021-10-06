using System;

namespace GarryDb
{
    public sealed class Square
    {
        private readonly string file;
        private readonly int rank;

        public Square(string file, int rank)
        {
            this.file = file;
            this.rank = rank;
        }

        public override bool Equals(object? obj)
        {
            return obj is Square that && file == that.file && rank == that.rank;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(file, rank);
        }

        public override string ToString()
        {
            return $"{file}{rank}";
        }
  
        public static bool operator ==(Square? sq, Square? s2)
        {
            if (ReferenceEquals(sq, null))
            {
                return false;
            }

            if (ReferenceEquals(s2, null))
            {
                return false;
            }

            return sq.Equals(s2);
        }

        public static bool operator !=(Square? s1, Square? s2)
        {
            return !(s1 == s2);
        }
    }
}
