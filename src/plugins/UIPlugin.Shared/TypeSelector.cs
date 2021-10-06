using System;

using Avalonia.Styling;

namespace UIPlugin.Shared
{
    /// <summary>
    ///     A selector that matches a type in the UIPlugin assembly.
    /// </summary>
    public sealed class TypeSelector : Selector
    {
        private readonly Lazy<Type> type;

        /// <summary>
        ///     Initializes a new <see cref="TypeSelector" />.
        /// </summary>
        /// <param name="typeName">The name of the type to select.</param>
        public TypeSelector(string typeName)
        {
            type = new Lazy<Type>(() => Type.GetType($"{typeName}, GarryDb.Avalonia.Host"));
        }

        /// <inheritdoc />
        protected override SelectorMatch Evaluate(IStyleable control, bool subscribe)
        {
            if (TargetType == null)
            {
                return SelectorMatch.NeverThisType;
            }

            Type controlType = control.StyleKey;
            return TargetType.IsAssignableFrom(controlType) || TargetType.IsInstanceOfType(control)
                ? SelectorMatch.AlwaysThisInstance
                : SelectorMatch.NeverThisInstance;
        }

        /// <inheritdoc />
        protected override Selector MovePrevious()
        {
            return null;
        }

        /// <inheritdoc />
        public override bool InTemplate
        {
            get { return false; }
        }

        /// <inheritdoc />
        public override bool IsCombinator
        {
            get { return false; }
        }

        /// <inheritdoc />
        public override Type TargetType
        {
            get { return type.Value; }
        }
    }
}
