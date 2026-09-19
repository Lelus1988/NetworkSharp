using System;
using System.Diagnostics;
using System.Windows;

namespace NetworkSharp.Views
{
    public partial class StartupGateWindow : Window
    {
        private bool _closedByButton;

        public StartupGateWindow(string heading, string message, string acceptText, string closeText)
        {
            InitializeComponent();
            HeadingText.Text = heading;
            MessageText.Text = message;
            AcceptButton.Content = acceptText;
            CloseButton.Content = closeText;
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            _closedByButton = true;
            DialogResult = true;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            _closedByButton = true;
            DialogResult = false;
        }

        private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_closedByButton)
                DialogResult = false;
        }

        private void RepositoryLink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/Lelus1988/NetworkSharp",
                UseShellExecute = true
            });
        }
    }
}