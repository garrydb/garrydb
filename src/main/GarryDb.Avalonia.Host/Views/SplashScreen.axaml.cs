using Avalonia;
using Avalonia.Controls;
using Avalonia.Diagnostics;
using Avalonia.Markup.Xaml;

namespace GarryDb.Avalonia.Host.Views
{
    public partial class SplashScreen : Window
    {
        public SplashScreen()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools(new DevToolsOptions());
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
