using System;
using System.Reactive.Subjects;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

using GarryDb.Avalonia.Host.Views;

using ReactiveUI;

using GarryDB.Plugins;

using UIPlugin.Shared;

namespace GarryDb.Avalonia.Host
{
    public sealed class UIPlugin : Plugin, IDisposable
    {
        private readonly ReplaySubject<Extension> extensions;

        public UIPlugin(PluginContext pluginContext)
            : base(pluginContext)
        {
            extensions = new ReplaySubject<Extension>();
            extensions.Subscribe(extension => CurrentApp.Extend(extension));

            Register<Extension>("extend", extension => extensions.OnNext(extension));
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
                    SendAsync("GarryPlugin", "shutdown").GetAwaiter().GetResult();
                }
            });
        }

        public void Dispose()
        {
            extensions.Dispose();
        }
    }
}
