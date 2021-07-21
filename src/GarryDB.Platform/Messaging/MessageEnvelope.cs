using GarryDb.Plugins;

namespace GarryDB.Platform.Messaging
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Address
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginIdentity"></param>
        /// <param name="handler"></param>
        public Address(PluginIdentity pluginIdentity, string handler)
        {
            PluginIdentity = pluginIdentity;
            Handler = handler;
        }

        /// <summary>
        /// 
        /// </summary>
        public PluginIdentity PluginIdentity { get; }

        /// <summary>
        /// 
        /// </summary>
        public string Handler { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Address CreateReturnAddress()
        {
            return new Address(PluginIdentity, Handler + "/reply");
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{PluginIdentity}.{Handler}";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class MessageEnvelope
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="destination"></param>
        /// <param name="message"></param>
        public MessageEnvelope(Address sender, Address destination, object message)
        {
            Sender = sender;
            Destination = destination;
            Message = message;
        }

        /// <summary>
        ///     Gets the address of the sender.
        /// </summary>
        public Address Sender { get; }

        /// <summary>
        /// 
        /// </summary>
        public Address Destination { get; }


        /// <summary>
        /// 
        /// </summary>
        public object Message { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public MessageEnvelope CreateReturnMessage(object message)
        {
            return new MessageEnvelope(Destination, Sender.CreateReturnAddress(), message);
        }
    }
}
