using ReactiveUI.Fody.Helpers;

namespace GarryDb.Avalonia.Host.ViewModels
{
    public sealed class SplashScreenViewModel : ViewModel
    {
        public SplashScreenViewModel()
        {
            Total = int.MaxValue;
            PluginsLoaded = 0;
            CurrentPlugin = string.Empty;
        }

        [Reactive]
        public string CurrentPlugin { get; set; }

        [Reactive]
        public int PluginsLoaded { get; set; }

        [Reactive]
        public int Total { get; set; }
    }
}
