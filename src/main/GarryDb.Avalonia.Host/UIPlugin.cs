using System;

using Avalonia;
using Avalonia.Controls;

using GarryDb.Avalonia.Host.Views;
using GarryDb.Plugins;

using UIPlugin.Shared;

namespace GarryDb.Avalonia.Host
{
    public sealed class UIPlugin : Plugin
    {
        public UIPlugin(PluginContext pluginContext)
            : base(pluginContext)
        {
            Register<Extension>("extend", extension => CurrentApp.ExtendAsync(extension));
        }

        private App CurrentApp
        {
            get { return (App)Application.Current; }
        }

        protected override void Start()
        {
            Window.WindowClosedEvent.Raised.Subscribe(tuple =>
            {
                if (tuple.Item1 is MainWindow)
                {
                    SendAsync("Garry", "shutdown").GetAwaiter().GetResult();
                }
            });

            CurrentApp.OnStarted();
        }
    }
}
