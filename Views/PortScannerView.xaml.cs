using System.Windows.Controls;
using NetworkSharp.ViewModels;

namespace NetworkSharp.Views
{
    /// <summary>
    /// Interaction logic for PortScannerView.xaml
    /// </summary>
    public partial class PortScannerView : UserControl
    {
        public PortScannerView()
        {
            InitializeComponent();
        }

        private async void StartScan_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PortScannerViewModel viewModel)
            {
                ProgressSection.Visibility = System.Windows.Visibility.Visible;
                await viewModel.StartScanAsync();
                ProgressSection.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private async void StopScan_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is PortScannerViewModel viewModel)
            {
                await viewModel.StopScanAsync();
                ProgressSection.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void PortRange_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string range && DataContext is PortScannerViewModel viewModel)
            {
                viewModel.SelectedPortRange = range;
            }
        }
    }
}
