using System;

namespace GarryDb.Plugins
{
    /// <summary>
    ///     Indicates the startup order of the plugin.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class StartupOrderAttribute : Attribute
    {
        /// <summary>
        ///     Initializes a new <see cref="StartupOrderAttribute" />.
        /// </summary>
        /// <param name="order">The order of the plugin.</param>
        public StartupOrderAttribute(int order)
        {
            Order = order;
        }

        /// <summary>
        ///     Gets the startup order.
        /// </summary>
        public int Order { get; }
    }
}
