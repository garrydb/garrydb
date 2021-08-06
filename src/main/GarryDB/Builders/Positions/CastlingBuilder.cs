using System.Collections.Generic;

using GarryDB.Platform.Extensions;

namespace GarryDB.Builders.Positions
{
    public abstract class CastlingBuilder<TBuilder> : ActiveColorBuilder<CastlingBuilder<TBuilder>>
        where TBuilder : CastlingBuilder<TBuilder>
    {
        protected CastlingBuilder()
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
