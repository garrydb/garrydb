using System;
using System.Collections.Generic;
using System.Linq;

using GarryDB.Platform.Extensions;

using Sprache;

namespace GarryDB.FEN
{
    public static class FenParser
    {
        private static readonly Func<char, Piece> CharToPieceConverter = c =>
                                                                         {
                                                                             Color color = char.IsUpper(c)
                                                                                 ? Color.White
                                                                                 : Color.Black;

                                                                             return Piece.Parse(c.ToString(), color);
                                                                         };

        private static readonly Parser<Color> ColorParser =
            from color in Parse.Chars('w', 'b')
            select color == 'w' ? Color.White : Color.Black;

        private static readonly Parser<Piece> CastleParser =
            Parse.Chars('K', 'Q', 'k', 'q').Select(piece => CharToPieceConverter(piece));

        private static readonly Parser<Square> SquareParser =
            from file in Parse.Chars(Enumerable.Range('a', 'h').Select(i => (char)i).ToArray()).Once().Text()
            from rank in Parse.Chars(Enumerable.Range('1', '8').Select(i => (char)i).ToArray()).Once().Text()
            select new Square(file, int.Parse(rank));

        private static readonly Parser<string> MoveNumberParser = Parse.Number;

        private static readonly Parser<IEnumerable<Piece>> EmptySquareParser =
            from amount in Parse.Chars(Enumerable.Range('1', '8').Select(i => (char)i).ToArray()).Once().Text()
            select Enumerable.Repeat('-', int.Parse(amount)).Select(_ => Piece.None);
            
        private static readonly Parser<Piece> PieceParser = Parse
                                                            .Chars('K', 'Q', 'R', 'B', 'N', 'P', 'k', 'q', 'r', 'b', 'n', 'p')
                                                            .Select(piece => CharToPieceConverter(piece));

        private static readonly Parser<IEnumerable<Piece>> RankParser =
                from pieces in PieceParser.Many().Or(EmptySquareParser).Until(Parse.Chars('/', ' ').Once())
                select pieces.SelectMany(x => x);

        private static readonly Parser<IEnumerable<IEnumerable<Piece>>> RanksParser = RankParser.Repeat(8);

        public static readonly Parser<ParsedPosition> Fen =
            from ranks in RanksParser
            from color in ColorParser
            from _ in Parse.WhiteSpace.Once()
            from castle in CastleParser.Repeat(1, 4).Or(Parse.Char('-').Select(_ => Enumerable.Empty<Piece>()))
            from __ in Parse.WhiteSpace.Once()
            from enPassant in SquareParser.Or(Parse.Char('-').Select(_ => default(Square)))
            from ___ in Parse.WhiteSpace.Once()
            from halfMoveNumber in MoveNumberParser
            from ____ in Parse.WhiteSpace.Once()
            from fullMoveNumber in MoveNumberParser
            select new ParsedPosition(CreatePosition(ranks.Reverse()),
                                      color,
                                      castle,
                                      enPassant,
                                      int.Parse(halfMoveNumber),
                                      int.Parse(fullMoveNumber));

        private static Position CreatePosition(IEnumerable<IEnumerable<Piece>> ranks)
        {
            var pieces =
                ranks.SelectMany((rank, rankNumber) => rank.Select((piece, fileNumber) => new
                                                                       {
                                                                           Square =
                                                                               new Square(char.ToString((char)(fileNumber + 'A')),
                                                                                   rankNumber + 1),
                                                                           Piece = piece
                                                                       }))
                     .ToDictionary(x => x.Square, x => x.Piece);

            return new Position(pieces);
        }

        public class ParsedPosition
        {
            public ParsedPosition(Position position, Color color, IEnumerable<Piece> castle, Square? enPassant, int halfMoveNumber, int fullMoveNumber)
            {
                Position = position;
                Color = color;
                Castle = castle;
                EnPassant = enPassant;
                HalfMoveNumber = halfMoveNumber;
                FullMoveNumber = fullMoveNumber;
            }

            public Position Position { get; }
            public Color Color { get; }
            public IEnumerable<Piece> Castle { get; }
            public Square? EnPassant { get; }
            public int HalfMoveNumber { get; }
            public int FullMoveNumber { get; }
        }
    }
}
