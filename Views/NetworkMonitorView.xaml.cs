using System.Windows.Controls;
using NetworkSharp.ViewModels;

namespace NetworkSharp.Views
{
    /// <summary>
    /// Interaction logic for NetworkMonitorView.xaml
    /// </summary>
    public partial class NetworkMonitorView : UserControl
    {
        public NetworkMonitorView()
        {
            InitializeComponent();
            
            this.Loaded += async (s, e) =>
            {
                if (DataContext is NetworkMonitorViewModel viewModel)
                {
                    // Start monitoring
                    await viewModel.StartMonitoringAsync();
                }
            };
            
            this.Unloaded += async (s, e) =>
            {
                if (DataContext is NetworkMonitorViewModel viewModel)
                {
                    await viewModel.StopMonitoringAsync();
                    viewModel.StopChartTimer();
                }
            };
        }

        private async void LiveButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is NetworkMonitorViewModel viewModel)
            {
                await viewModel.UpdateChartDataAsync("Live");
            }
        }

        private async void Button24h_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is NetworkMonitorViewModel viewModel)
            {
                await viewModel.UpdateChartDataAsync("24 Std");
            }
        }

        private async void Button7d_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is NetworkMonitorViewModel viewModel)
            {
                await viewModel.UpdateChartDataAsync("7 Tage");
            }
        }

        private async void Button30d_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is NetworkMonitorViewModel viewModel)
            {
                await viewModel.UpdateChartDataAsync("30 Tage");
            }
        }
    }
}
