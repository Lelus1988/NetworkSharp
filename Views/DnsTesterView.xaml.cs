using System.Windows.Controls;
using NetworkSharp.ViewModels;

namespace NetworkSharp.Views
{
    /// <summary>
    /// Interaction logic for DnsTesterView.xaml
    /// </summary>
    public partial class DnsTesterView : UserControl
    {
        public DnsTesterView()
        {
            InitializeComponent();
        }

        private async void StartTest_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is DnsTesterViewModel viewModel)
            {
                await viewModel.StartTestAsync();
            }
        }
    }
}
