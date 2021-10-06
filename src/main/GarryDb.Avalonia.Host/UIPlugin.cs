using System;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

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

        protected override Task StartAsync()
        {
            Window.WindowClosedEvent.Raised.Subscribe(tuple =>
            {
                if (tuple.Item1 is MainWindow)
                {
                    SendAsync("Garry", "shutdown").GetAwaiter().GetResult();
                }
            });

            return Dispatcher.UIThread.InvokeAsync(() => CurrentApp.OnStarted());
        }
    }
}
