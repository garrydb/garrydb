﻿using System;

using GarryDB.Platform.Messaging;
using GarryDB.Platform.Plugins;
using GarryDB.Specs.Platform.Plugins.Builders;

namespace GarryDB.Specs.Platform.Messaging.Builders
{
    public sealed class MessageEnvelopeBuilder : TestDataBuilder<MessageEnvelope>
    {
        private PluginIdentity sender;
        private Address destination;
        private object message;

        protected override void OnPreBuild()
        {
            if (sender == null)
            {
                SentFrom(new PluginIdentityBuilder().Build());
            }

            if (destination == null)
            {
                AddressedTo(new AddressBuilder().Build());
            }

            if (message == null)
            {
                ContainingMessage(Guid.NewGuid());
            }
        }

        protected override MessageEnvelope OnBuild()
        {
            return new MessageEnvelope(sender, destination, message);
        }

        public MessageEnvelopeBuilder SentFrom(PluginIdentity sender)
        {
            this.sender = sender;
            return this;
        }

        public MessageEnvelopeBuilder AddressedTo(PluginIdentity pluginIdentity, string handler)
        {
            return AddressedTo(new Address(pluginIdentity, handler));
        }

        public MessageEnvelopeBuilder AddressedTo(Address destination)
        {
            this.destination = destination;
            return this;
        }

        public MessageEnvelopeBuilder ContainingMessage(object message)
        {
            this.message = message;
            return this;
        }
    }
}