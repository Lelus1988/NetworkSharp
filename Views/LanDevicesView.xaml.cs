using System.Windows.Controls;
using NetworkSharp.ViewModels;

namespace NetworkSharp.Views
{
    /// <summary>
    /// Interaction logic for LanDevicesView.xaml
    /// </summary>
    public partial class LanDevicesView : UserControl
    {
        public LanDevicesView()
        {
            InitializeComponent();
        }

        private async void StartScan_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is LanDevicesViewModel viewModel)
            {
                await viewModel.StartScanAsync();
            }
        }
    }
}
