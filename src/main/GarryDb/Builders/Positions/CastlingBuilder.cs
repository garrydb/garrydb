using System.Collections.Generic;

namespace GarryDb.Builders.Positions
{
    public abstract class CastlingBuilder<TBuilder> : ActiveColorBuilder<CastlingBuilder<TBuilder>>
        where TBuilder : CastlingBuilder<TBuilder>
    {
        protected CastlingBuilder()
        {
            CastlingPossibilities = new List<Piece>();
        }
        
        public TBuilder WithCastlingAvalability(params Piece[] pieces)
        {
            foreach (Piece piece in pieces)
            {
                CastlingPossibilities.Add(piece);
            }

            return (TBuilder)this;
        }
    }
}
