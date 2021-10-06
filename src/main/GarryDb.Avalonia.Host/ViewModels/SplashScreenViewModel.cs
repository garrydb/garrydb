using ReactiveUI;

namespace GarryDb.Avalonia.Host.ViewModels
{
    public sealed class SplashScreenViewModel : ViewModel
    {
        private string currentPlugin;
        private int pluginsLoaded;
        private int total;

        public SplashScreenViewModel()
        {
            total = int.MaxValue;
            pluginsLoaded = 0;
            currentPlugin = string.Empty;
        }

        public string CurrentPlugin
        {
            get { return currentPlugin; }
            set { this.RaiseAndSetIfChanged(ref currentPlugin, value); }
        }

        public int PluginsLoaded
        {
            get { return pluginsLoaded; }
            set { this.RaiseAndSetIfChanged(ref pluginsLoaded, value); }
        }

        public int Total
        {
            get { return total; }
            set { this.RaiseAndSetIfChanged(ref total, value); }
        }
    }
}
