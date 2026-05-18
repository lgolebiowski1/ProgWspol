using System.Windows;
using System.Windows.Controls;
using TP.ConcurrentProgramming.Presentation.ViewModel;

namespace TP.ConcurrentProgramming.PresentationView
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TableBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DataContext is MainWindowViewModel vm && sender is Border border)
            {
                double thickness = border.BorderThickness.Left + border.BorderThickness.Right;
                double thicknessV = border.BorderThickness.Top + border.BorderThickness.Bottom;
                vm.CanvasWidth = Math.Max(1, border.ActualWidth - thickness);
                vm.CanvasHeight = Math.Max(1, border.ActualHeight - thicknessV);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
                viewModel.Dispose();
            base.OnClosed(e);
        }
    }
}
