using System.Windows;

using GarryDB.Platform.Plugins;

namespace GarryDB.Wpf.Host
{
    /// <summary>
    ///     Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen
    {
        public static readonly DependencyProperty CurrentPluginProperty =
            DependencyProperty.Register("CurrentPlugin", typeof(PluginIdentity), typeof(SplashScreen),
                                        new PropertyMetadata(default(PluginIdentity)));

        public static readonly DependencyProperty CurrentProperty =
            DependencyProperty.Register("Current", typeof(int), typeof(SplashScreen), new PropertyMetadata(default(int), OnProgressChanged));

        public static readonly DependencyProperty TotalProperty =
            DependencyProperty.Register("Total", typeof(int), typeof(SplashScreen), new PropertyMetadata(int.MaxValue));

        public static readonly DependencyProperty PhaseProperty =
            DependencyProperty.Register("Phase", typeof(string), typeof(SplashScreen), new PropertyMetadata("Loading..."));

        private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var splashScreen = (SplashScreen)d;

            if (splashScreen.Current == splashScreen.Total - 1)
            {
                splashScreen.Phase = "Starting...";
            }
            else if (splashScreen.Current == splashScreen.Total)
            {
                splashScreen.Close();
            }
        }

        public SplashScreen()
        {
            InitializeComponent();
        }

        public PluginIdentity CurrentPlugin
        {
            get { return (PluginIdentity)GetValue(CurrentPluginProperty); }
            set { SetValue(CurrentPluginProperty, value); }
        }

        public int Current
        {
            get { return (int)GetValue(CurrentProperty); }
            set { SetValue(CurrentProperty, value); }
        }

        public int Total
        {
            get { return (int)GetValue(TotalProperty); }
            set { SetValue(TotalProperty, value); }
        }

        public string Phase
        {
            get { return (string)GetValue(PhaseProperty); }
            private set { SetValue(PhaseProperty, value); }
        }
    }
}
