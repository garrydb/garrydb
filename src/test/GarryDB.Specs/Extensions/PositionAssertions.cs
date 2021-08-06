using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Formatting;
using FluentAssertions.Primitives;

using GarryDB.Pieces;

namespace GarryDB.Specs.Extensions
{
    internal sealed class PositionAssertions : ReferenceTypeAssertions<Position, PositionAssertions>
    {
        static PositionAssertions()
        {
            Formatter.AddFormatter(new PositionFormatter());
        }

        private static readonly FieldInfo PiecesField =
            typeof(Position).GetField("pieces", BindingFlags.GetField | BindingFlags.Instance | BindingFlags.NonPublic);

        public PositionAssertions(Position subject)
            : base(subject)
        {
        }

        protected override string Identifier
        {
            get { return "position"; }
        }

        [CustomAssertion]
        public AndConstraint<PositionAssertions> Have64Squares(string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                   .BecauseOf(because, becauseArgs)
                   .Given(() => (IDictionary<Square, Piece>)PiecesField.GetValue(Subject))
                   .ForCondition(pieces => pieces.Count == 64)
                   .FailWith("Expected {context:position} to be have 64 squares{reason}, but was {0}", squares => squares.Count);

            return new AndConstraint<PositionAssertions>(this);
        }

        [CustomAssertion]
        public AndConstraint<PositionAssertions> BeEmpty(string because = "", params object[] becauseArgs)
        {
            Execute.Assertion
                   .BecauseOf(because, becauseArgs)
                   .Given(() => (IDictionary<Square, Piece>)PiecesField.GetValue(Subject))
                   .ForCondition(pieces => pieces.Values.All(piece => piece is None))
                   .FailWith("Expected {context:position} to be empty{reason}, but contained {0}", Subject);

            return new AndConstraint<PositionAssertions>(this);
        }
    }
}
