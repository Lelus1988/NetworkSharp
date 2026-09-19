using System.Windows.Controls;
using NetworkSharp.ViewModels;

namespace NetworkSharp.Views
{
    /// <summary>
    /// Interaction logic for IpGeoIpView.xaml
    /// </summary>
    public partial class IpGeoIpView : UserControl
    {
        public IpGeoIpView()
        {
            InitializeComponent();
        }

        private async void Refresh_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is IpGeoIpViewModel viewModel)
            {
                await viewModel.RefreshAsync();
            }
        }
    }
}
