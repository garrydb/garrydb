using System;
using System.Windows;
using System.Windows.Threading;

namespace GarryDB.Wpf.Host
{
    /// <summary>
    ///     Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen
    {
        public static readonly DependencyProperty CurrentPluginProperty =
            DependencyProperty.Register("CurrentPlugin", typeof(string), typeof(SplashScreen),
                                        new PropertyMetadata(default(string)));

        public static readonly DependencyProperty LoadingProperty =
            DependencyProperty.Register("Loading", typeof(int), typeof(SplashScreen), new PropertyMetadata(default(int), OnProgressChanged));
        
        public static readonly DependencyProperty PluginsLoadedProperty = DependencyProperty.Register(
            "PluginsLoaded", typeof(int), typeof(SplashScreen), new PropertyMetadata(default(int), OnProgressChanged));

        public static readonly DependencyProperty TotalProperty =
            DependencyProperty.Register("Total", typeof(int), typeof(SplashScreen), new PropertyMetadata(int.MaxValue));

        public static readonly DependencyProperty PhaseProperty =
            DependencyProperty.Register("Phase", typeof(string), typeof(SplashScreen), new PropertyMetadata("Loading..."));

        private static void OnProgressChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var splashScreen = (SplashScreen)d;

            if (splashScreen.PluginsLoaded == splashScreen.Total)
            {
                splashScreen.Phase = "Starting...";

                var timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };

                timer.Tick += (_, _) =>
                {
                    splashScreen.Close();
                    timer.Stop();
                };

                timer.Start();
            }
        }

        public SplashScreen()
        {
            InitializeComponent();
        }

        public string CurrentPlugin
        {
            get { return (string)GetValue(CurrentPluginProperty); }
            set { SetValue(CurrentPluginProperty, value); }
        }

        public int Loading
        {
            get { return (int)GetValue(LoadingProperty); }
            set { SetValue(LoadingProperty, value); }
        }

        public int PluginsLoaded
        {
            get { return (int)GetValue(PluginsLoadedProperty); }
            set { SetValue(PluginsLoadedProperty, value); }
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
