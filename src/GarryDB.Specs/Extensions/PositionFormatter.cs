using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using FluentAssertions.Formatting;

namespace GarryDB.Specs.Extensions
{
    public class PositionFormatter : IValueFormatter
    {
        private static readonly FieldInfo PiecesField =
            typeof(Position).GetField("pieces", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);

        public bool CanHandle(object value)
        {
            return value is Position;
        }

        public void Format(object value, FormattedObjectGraph formattedGraph, FormattingContext context, FormatChild formatChild)
        {
            StringBuilder stringBuilder =
                new StringBuilder()
                    .Append("  +")
                    .AppendJoin(null, Enumerable.Repeat("---+", 8))
                    .AppendLine();

            var pieces = (IDictionary<Square, Piece>)PiecesField.GetValue(value);
            foreach (int rank in Enumerable.Range(1, 8).Reverse())
            {
                stringBuilder.Append(rank)
                             .Append(" |");

                foreach (string file in Enumerable.Range('a', 8).Select(i => char.ToString((char)i)))
                {
                    Piece piece = pieces[new Square(file, rank)];
                    stringBuilder.Append($" {piece} |");
                }

                stringBuilder.AppendLine();
                stringBuilder.Append("  +");
                stringBuilder.AppendJoin(null, Enumerable.Repeat("---+", 8))
                             .AppendLine();
            }

            stringBuilder.Append("  | ");
            stringBuilder.AppendJoin(" | ", Enumerable.Range('a', 8).Select(i => char.ToString((char)i)));
            stringBuilder.Append(" |");

            formattedGraph.AddFragmentOnNewLine(stringBuilder.ToString());
        }
    }
}
