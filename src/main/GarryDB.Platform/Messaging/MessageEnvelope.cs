using GarryDb.Platform.Plugins;

namespace GarryDb.Platform.Messaging
{
    /// <summary>
    ///     Wrapped around a message and contains metadata about that message.
    /// </summary>
    internal sealed class MessageEnvelope
    {
        private readonly PluginIdentity sender;

        /// <summary>
        ///     Initializes a new <see cref="MessageEnvelope" />.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="destination">The destination of the message.</param>
        public MessageEnvelope(PluginIdentity sender, Address destination)
            : this(sender, destination, new object())
        {
        }

        /// <summary>
        ///     Initializes a new <see cref="MessageEnvelope" />.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        /// <param name="destination">The destination of the message.</param>
        /// <param name="message">The message.</param>
        public MessageEnvelope(PluginIdentity sender, Address destination, object message)
        {
            this.sender = sender;
            Destination = destination;
            Message = message;
        }

        /// <summary>
        ///     Gets the destination of the message.
        /// </summary>
        public Address Destination { get; }

        /// <summary>
        ///     Gets the message.
        /// </summary>
        public object Message { get; }

        /// <summary>
        ///     Create a <see cref="MessageEnvelope" /> containing a response for the original sender.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns>A <see cref="MessageEnvelope" /> containing the message and is addressed to the sender.</returns>
        public MessageEnvelope CreateReturnMessage(object message)
        {
            return new MessageEnvelope(Destination.PluginIdentity, new Address(sender, $"{Destination.Handler}/reply"), message);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{sender} -> {Destination}";
        }
    }
}
