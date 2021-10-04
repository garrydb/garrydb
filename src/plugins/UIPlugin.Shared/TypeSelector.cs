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

        public TypeSelector(string typeName)
        {
            type = new Lazy<Type>(() => Type.GetType($"{typeName}, GarryDb.Avalonia.Host"));
        }

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

        protected override Selector MovePrevious()
        {
            return null;
        }

        public override bool InTemplate
        {
            get { return false; }
        }

        public override bool IsCombinator
        {
            get { return false; }
        }

        public override Type TargetType
        {
            get { return type.Value; }
        }
    }
}
