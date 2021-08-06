using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GarryDB.UI.Views
{
    public partial class ChessboardView : UserControl
    {
        public ChessboardView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
