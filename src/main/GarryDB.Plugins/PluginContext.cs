using System.Threading.Tasks;

namespace GarryDb.Plugins
{
    /// <summary>
    ///     Sends messages to other plugins.
    /// </summary>
    public interface PluginContext
    {
        /// <summary>
        ///     Sends <paramref name="message" /> to <paramref name="destination" />.
        /// </summary>
        /// <param name="destination">The destination plugin.</param>
        /// <param name="handler">The name of the handler.</param>
        /// <param name="message">The message.</param>
        Task SendAsync(string destination, string handler, object message);
    }
}
