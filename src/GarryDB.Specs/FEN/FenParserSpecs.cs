using GarryDB.Fen;
using GarryDB.Specs.Extensions;

using NUnit.Framework;

using Sprache;

namespace GarryDB.Specs.Fen
{
    [TestFixture]
    public class FenParserSpecs
    {
        [TestCase("8/8/8/8/8/8/8/8 w KQkq - 0 1")]
        [TestCase("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")]
        [TestCase("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQ e3 0 1")]
        [TestCase("rnbqkbnr/pp1ppppp/8/2p5/4P3/8/PPPP1PPP/RNBQKBNR w - c6 0 2")]
        [TestCase("7B/8/4p3/2knq2R/8/4PP2/B7/5K2 b KQkq - 1 2")]
        [TestCase("r4rk1/5ppp/p4n2/1p1q1QB1/3b4/N7/PP3PPP/1R3RK1 w - - 1 0")]
        public void Test(string fen)
        {
            Position position = FenParser.Fen.Parse(fen);

            position.Should().Have64Squares();
        }

        [TestFixture]
        public class When_parsing_a_fen_representing_an_empty_board : Specification
        {
            private string fen;
            private Position position;

            protected override void Given()
            {
                fen = "8/8/8/8/8/8/8/8 w KQkq - 0 1";
            }

            protected override void When()
            {
                position = FenParser.Fen.Parse(fen);
            }

            [Test]
            public void It_should_be_empty()
            {
                position.Should().BeEmpty();
            }

            [Test]
            public void It_should_contain_the_complete_board()
            {
                position.Should().Have64Squares();
            }
        }
    }
}
