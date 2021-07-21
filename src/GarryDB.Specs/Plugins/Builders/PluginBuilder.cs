using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GarryDb.Plugins;

namespace GarryDb.Specs.Plugins.Builders
{
    public sealed class PluginBuilder : TestDataBuilder<Plugin>
    {
        private readonly IDictionary<string, dynamic> registrations = new Dictionary<string, dynamic>();
        private PluginContext pluginContext;

        protected override Plugin OnBuild()
        {
            return new PluginStub(this);
        }
        
        public PluginBuilder Register<TMessage>(string name, Action<TMessage> handler)
        {
            return Register(name, (TMessage message) =>
                                  {
                                      handler(message);
                                      return default(object);
                                  });
        }

        public PluginBuilder Register<TMessage, TResult>(string name, Func<TMessage, TResult> handler)
        {
            return Register(name, (TMessage message) =>
                                  {
                                      TResult result = handler(message);
                                      return Task.FromResult(result);
                                  });
        }

        public PluginBuilder Register<TMessage, TResult>(string name, Func<TMessage, Task<TResult>> handler)
        {
            registrations[name] = handler;
            return this;
        }

        private class PluginStub : Plugin
        {
            public PluginStub(PluginBuilder builder)
                : base(builder.pluginContext)
            {
                foreach (KeyValuePair<string, dynamic> registration in builder.registrations)
                {
                    Register(registration.Key, registration.Value);
                }
            }
        }
    }
}
