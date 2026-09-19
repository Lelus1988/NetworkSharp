using System.Windows.Controls;
using NetworkSharp.ViewModels;

namespace NetworkSharp.Views
{
    /// <summary>
    /// Interaction logic for PingView.xaml
    /// </summary>
    public partial class PingView : UserControl
    {
        private PingViewModel? _viewModel;
        private bool _isStarted = false;

        public PingView()
        {
            InitializeComponent();

            this.Loaded += async (s, e) =>
            {
                try
                {
                    if (DataContext is PingViewModel viewModel)
                    {
                        if (_viewModel == null)
                        {
                            _viewModel = viewModel;
                        }

                        if (!_isStarted)
                        {
                            await _viewModel.StartPingAsync();
                            _isStarted = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error starting ping: {ex.Message}");
                }
            };

            this.Unloaded += async (s, e) =>
            {
                try
                {
                    if (_viewModel != null && _isStarted)
                    {
                        await _viewModel.StopPingAsync();
                        _viewModel.Dispose();
                        _viewModel = null;
                        _isStarted = false;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error stopping ping: {ex.Message}");
                }
            };
        }

        private async void StopPing_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                if (_viewModel != null)
                {
                    await _viewModel.StopPingAsync();
                    _isStarted = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error stopping ping: {ex.Message}");
            }
        }
    }
}
