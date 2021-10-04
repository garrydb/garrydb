using ReactiveUI;

namespace GarryDb.Avalonia.Host.ViewModels
{
    public sealed class SplashScreenViewModel : ViewModel
    {
        private string currentPlugin;
        private int loading;
        private int pluginsLoaded;
        private int total;
        private string phase;

        public SplashScreenViewModel()
        {
            phase = "Loading...";
            total = int.MaxValue;
        }

        public string CurrentPlugin
        {
            get { return currentPlugin; }
            set { this.RaiseAndSetIfChanged(ref currentPlugin, value); }
        }

        public int Loading
        {
            get { return loading; }
            set { this.RaiseAndSetIfChanged(ref loading, value); }
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

        public string Phase
        {
            get { return phase; }
            set { this.RaiseAndSetIfChanged(ref phase, value); }
        }
    }
}
