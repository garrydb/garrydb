using System.Collections.Generic;

using GarryDB.Platform.Extensions;

namespace GarryDB.Builders.Positions
{
    public class PositionCastlingBuilder<TBuilder> : PositionActiveColorBuilder<PositionCastlingBuilder<TBuilder>>
        where TBuilder : PositionCastlingBuilder<TBuilder>
    {
        protected PositionCastlingBuilder()
        {
            castlingPossibilities = new List<Piece>();
        }
        
        public TBuilder WithCastlingAvalability(params Piece[] pieces)
        {
            pieces.ForEach(piece => castlingPossibilities.Add(piece));

            return (TBuilder)this;
        }
    }
}
