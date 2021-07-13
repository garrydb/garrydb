using System.Windows;

using GarryDb.Platform.Plugins;

namespace GarryDB.Wpf.Host
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen
    {
        public SplashScreen()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty NumberOfStepsProperty = DependencyProperty.Register(
            "NumberOfSteps", typeof(int), typeof(SplashScreen), new PropertyMetadata(int.MaxValue));

        public int NumberOfSteps
        {
            get { return (int) GetValue(NumberOfStepsProperty); }
            set { SetValue(NumberOfStepsProperty, value); }
        }

        public static readonly DependencyProperty CurrentStepProperty = DependencyProperty.Register(
            "CurrentStep", typeof(int), typeof(SplashScreen), new PropertyMetadata(default(int)));

        public int CurrentStep
        {
            get { return (int) GetValue(CurrentStepProperty); }
            set { SetValue(CurrentStepProperty, value); }
        }

        public static readonly DependencyProperty NumberOfPluginsProperty = DependencyProperty.Register(
            "NumberOfPlugins", typeof(int), typeof(SplashScreen), new PropertyMetadata(default(int)));

        public int NumberOfPlugins
        {
            get { return (int) GetValue(NumberOfPluginsProperty); }
            set { SetValue(NumberOfPluginsProperty, value); }
        }

        public static readonly DependencyProperty NumberOfPluginsStartedProperty = DependencyProperty.Register(
            "NumberOfPluginsStarted", typeof(int), typeof(SplashScreen), new PropertyMetadata(default(int)));

        public int NumberOfPluginsStarted
        {
            get { return (int) GetValue(NumberOfPluginsStartedProperty); }
            set { SetValue(NumberOfPluginsStartedProperty, value); }
        }

        public static readonly DependencyProperty PhaseProperty = DependencyProperty.Register(
            "Phase", typeof(string), typeof(SplashScreen), new PropertyMetadata("Loading"));

        public string Phase
        {
            get { return (string) GetValue(PhaseProperty); }
            set { SetValue(PhaseProperty, value); }
        }

        public static readonly DependencyProperty CurrentPluginProperty = DependencyProperty.Register(
            "CurrentPlugin", typeof(PluginIdentity), typeof(SplashScreen), new PropertyMetadata(default(PluginIdentity)));

        public PluginIdentity CurrentPlugin
        {
            get { return (PluginIdentity) GetValue(CurrentPluginProperty); }
            set { SetValue(CurrentPluginProperty, value); }
        }
    }
}
