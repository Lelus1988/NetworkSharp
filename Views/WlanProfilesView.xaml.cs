using System.Windows.Controls;
using System.Windows.Input;
using NetworkSharp.ViewModels;
using NetworkSharp.Services;

namespace NetworkSharp.Views
{
    /// <summary>
    /// Interaction logic for WlanProfilesView.xaml
    /// </summary>
    public partial class WlanProfilesView : UserControl
    {
        public WlanProfilesView()
        {
            InitializeComponent();
        }

        private void Profile_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is WlanProfile profile && DataContext is WlanProfilesViewModel viewModel)
            {
                viewModel.SelectedProfile = profile;
            }
        }

        private async void ShowPassword_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is WlanProfilesViewModel viewModel)
            {
                await viewModel.ShowPasswordAsync();
            }
        }

        private void HidePassword_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is WlanProfilesViewModel viewModel)
            {
                viewModel.HidePassword();
            }
        }

        private async void CopyPassword_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is WlanProfilesViewModel viewModel)
            {
                await viewModel.CopyPasswordAsync();
            }
        }

        private async void DeleteProfile_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is WlanProfilesViewModel viewModel)
            {
                await viewModel.DeleteProfileAsync();
            }
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox && DataContext is WlanProfilesViewModel viewModel)
            {
                viewModel.Password = passwordBox.Password;
            }
        }
    }
}
