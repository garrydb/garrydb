using GarryDB.FEN;

using NUnit.Framework;

using Sprache;

namespace GarryDB.Specs.FEN
{
    [TestFixture]
    public class FenParserSpecs
    {
        [Test]
        public void Test()
        {
            var p1 = FenParser.Fen.Parse("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
            var p2 = FenParser.Fen.Parse("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQ e3 0 1");
            var p3 = FenParser.Fen.Parse("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w - c6 0 2");
            var p4 = FenParser.Fen.Parse("7B/8/4p3/2knq2R/8/4PP2/B7/5K2 b KQkq - 1 2");
            var p5 = FenParser.Fen.Parse("r4rk1/5ppp/p4n2/1p1q1QB1/3b4/N7/PP3PPP/1R3RK1 w - - 1 0");
        }
    }
}
