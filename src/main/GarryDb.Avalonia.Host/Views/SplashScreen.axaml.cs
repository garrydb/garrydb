using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GarryDb.Avalonia.Host.Views
{
    public partial class SplashScreen : Window
    {
        public static readonly StyledProperty<string> CurrentPluginProperty =
            AvaloniaProperty.Register<SplashScreen, string>(nameof(CurrentPlugin));

        public static readonly StyledProperty<int> LoadingProperty =
            AvaloniaProperty.Register<SplashScreen, int>(nameof(Loading));
        
        public static readonly StyledProperty<int> PluginsLoadedProperty =
            AvaloniaProperty.Register<SplashScreen, int>(nameof(PluginsLoaded));

        public static readonly StyledProperty<int> TotalProperty =
            AvaloniaProperty.Register<SplashScreen, int>(nameof(Total), int.MaxValue);

        public static readonly StyledProperty<string> PhaseProperty =
            AvaloniaProperty.Register<SplashScreen, string>(nameof(Phase), "Loading...");

        public SplashScreen()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            var splashScreen = (SplashScreen)change.Sender;

            if (splashScreen.PluginsLoaded == splashScreen.Total)
            {
                splashScreen.CurrentPlugin = string.Empty;
                splashScreen.Phase = "Starting...";
            }
        }
        
        public string CurrentPlugin
        {
            get { return GetValue(CurrentPluginProperty); }
            set { SetValue(CurrentPluginProperty, value); }
        }

        public int Loading
        {
            get { return GetValue(LoadingProperty); }
            set { SetValue(LoadingProperty, value); }
        }

        public int PluginsLoaded
        {
            get { return GetValue(PluginsLoadedProperty); }
            set { SetValue(PluginsLoadedProperty, value); }
        }

        public int Total
        {
            get { return GetValue(TotalProperty); }
            set { SetValue(TotalProperty, value); }
        }

        public string Phase
        {
            get { return GetValue(PhaseProperty); }
            private set { SetValue(PhaseProperty, value); }
        }
    }
}
