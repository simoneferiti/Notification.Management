using Microsoft.UI.Xaml;
using Notification.Core.Models;
using Notification.UI.ViewModels;
using System;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Notification.UI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; }

        // Usato dal ComboBox delle priorità in XAML.
        public NotificationPriority[] Priorities { get; } =
            (NotificationPriority[])Enum.GetValues(typeof(NotificationPriority));

        public MainWindow(MainViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var messageDialog = new MessageDialog("No internet connection has been found.");
            await messageDialog.ShowAsync();

        }
    }
}
