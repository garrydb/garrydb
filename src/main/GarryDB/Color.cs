namespace GarryDB
{
    public sealed class Color
    {
        public static readonly Color White = new Color("white");
        public static readonly Color Black = new Color("black");
        public static readonly Color None = new Color("none");

        private readonly string name;

        private Color(string name)
        {
            this.name = name;
        }

        public override bool Equals(object? obj)
        {
            return obj is Color that && name == that.name;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public override string ToString()
        {
            return name;
        }
   
        public static bool operator ==(Color? c1, Color? c2)
        {
            if (ReferenceEquals(c1, null))
            {
                return false;
            }

            if (ReferenceEquals(c2, null))
            {
                return false;
            }

            return c1.Equals(c2);
        }

        public static bool operator !=(Color? c1, Color? c2)
        {
            return !(c1 == c2);
        }
    }
}
