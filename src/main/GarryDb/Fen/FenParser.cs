using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using GarryDb.Pieces;

using Sprache;

namespace GarryDb.Fen
{
    internal static class FenParser
    {
        private static readonly Func<char, Piece> CharToPieceConverter = c =>
        {
            Color color = char.IsUpper(c)
                ? Color.White
                : Color.Black;

            switch (char.ToUpperInvariant(c))
            {
                case 'K':
                    return new King(color);
                case 'Q':
                    return new Queen(color);
                case 'R':
                    return new Rook(color);
                case 'B':
                    return new Bishop(color);
                case 'N':
                    return new Knight(color);
                case 'P':
                    return new Pawn(color);
                default:
                    return new None();
            }
        };

        private static readonly Parser<Color> ColorParser =
            from color in Parse.Chars('w', 'b')
            select color == 'w' ? Color.White : Color.Black;

        private static readonly Parser<Piece> CastleParser =
            Parse.Chars('K', 'Q', 'k', 'q').Select(piece => CharToPieceConverter(piece));

        private static readonly Parser<Square> SquareParser =
            from file in Parse.Chars(Enumerable.Range('a', 'h').Select(i => (char)i).ToArray()).Once().Text()
            from rank in Parse.Chars(Enumerable.Range('1', '8').Select(i => (char)i).ToArray()).Once().Text()
            select new Square(file, int.Parse(rank, CultureInfo.InvariantCulture));

        private static readonly Parser<string> MoveNumberParser = Parse.Number;

        private static readonly Parser<IEnumerable<Piece>> EmptySquareParser =
            from amount in Parse.Chars(Enumerable.Range('1', '8').Select(i => (char)i).ToArray()).Once().Text()
            select Enumerable.Repeat('-', int.Parse(amount, CultureInfo.InvariantCulture)).Select(_ => new None());

        private static readonly Parser<Piece> PieceParser = Parse
            .Chars('K', 'Q', 'R', 'B', 'N', 'P', 'k', 'q', 'r', 'b', 'n', 'p')
            .Select(piece => CharToPieceConverter(piece));

        private static readonly Parser<IEnumerable<Piece>> RankParser =
            from pieces in PieceParser.Many().Or(EmptySquareParser).Until(Parse.Chars('/', ' ').Once())
            select pieces.SelectMany(x => x);

        private static readonly Parser<IEnumerable<IEnumerable<Piece>>> RanksParser = RankParser.Repeat(8);

        public static readonly Parser<Position> Fen =
            from ranks in RanksParser
            from activeColor in ColorParser
            from _ in Parse.WhiteSpace.Once()
            from castlingAvailabilities in CastleParser.Repeat(1, 4).Or(Parse.Char('-').Select(_ => Enumerable.Empty<Piece>()))
            from __ in Parse.WhiteSpace.Once()
            from enPassantSquare in SquareParser.Or(Parse.Char('-').Select(_ => default(Square)))
            from ___ in Parse.WhiteSpace.Once()
            from halfMoveNumber in MoveNumberParser
            from ____ in Parse.WhiteSpace.Once()
            from fullMoveNumber in MoveNumberParser
            select CreatePosition(
                ranks.Reverse(),
                activeColor,
                castlingAvailabilities,
                enPassantSquare,
                int.Parse(halfMoveNumber, CultureInfo.InvariantCulture),
                int.Parse(fullMoveNumber, CultureInfo.InvariantCulture));

        private static Position CreatePosition(IEnumerable<IEnumerable<Piece>> ranks,
            Color activeColor,
            IEnumerable<Piece> castlingAvailabilities,
            Square enPassantSquare,
            int halfMoveNumber,
            int fullMoveNumber
        )
        {
            IDictionary<Square, Piece> pieces =
                ranks.SelectMany((rank, rankNumber) => rank.Select((piece, fileNumber) =>
                    {
                        string file = char.ToString((char)(fileNumber + 'a'));

                        return new
                        {
                            Square = new Square(file, rankNumber + 1),
                            Piece = piece
                        };
                    }))
                    .ToDictionary(x => x.Square, x => x.Piece);

            return
                Position.New()
                    .WithPieces(pieces)
                    .WithActiveColor(activeColor)
                    .WithCastlingAvalability(castlingAvailabilities.ToArray())
                    .WithEnPassantSquare(enPassantSquare)
                    .WitHalfMoveClock(halfMoveNumber)
                    .WithFullMove(fullMoveNumber)
                    .Build();
        }
    }
}
