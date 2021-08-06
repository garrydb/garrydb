using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using GarryDB.Plugins;

namespace GarryDB.Specs.Plugins.Builders
{
    public sealed class PluginBuilder : TestDataBuilder<Plugin>
    {
        private readonly IDictionary<string, dynamic> registrations = new Dictionary<string, dynamic>();
        private Func<Plugin> factory;
        private Action<object> onConfiguring;
        private PluginContext pluginContext;

        protected override void OnPreBuild()
        {
            if (pluginContext == null)
            {
                Using(new PluginContextBuilder().Build());
            }

            if (factory == null)
            {
                WhenConfiguring(() =>
                                {
                                });
            }
        }

        protected override Plugin OnBuild()
        {
            return factory();
        }

        public PluginBuilder WhenConfiguring(Action onConfigure)
        {
            return WhenConfiguring<object>(_ => onConfigure());
        }

        public PluginBuilder WhenConfiguring<TConfiguration>(Action<TConfiguration> onConfigure) where TConfiguration : new()
        {
            onConfiguring = o => onConfigure((TConfiguration)o);
            factory = () => new PluginStub<TConfiguration>(this);

            return this;
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

        public PluginBuilder Using(PluginContext pluginContext)
        {
            this.pluginContext = pluginContext;

            return this;
        }

        private class PluginStub<TConfiguration> : ConfigurablePlugin<TConfiguration> where TConfiguration : new()
        {
            private readonly PluginBuilder builder;

            public PluginStub(PluginBuilder builder)
                : base(builder.pluginContext)
            {
                this.builder = builder;
                foreach (KeyValuePair<string, dynamic> registration in builder.registrations)
                {
                    Register(registration.Key, registration.Value);
                }
            }

            protected override void Configure(TConfiguration configuration)
            {
                builder.onConfiguring(configuration);
            }
        }
    }
}
