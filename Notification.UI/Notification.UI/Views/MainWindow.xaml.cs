using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Notification.Core.Models;
using Notification.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

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
    }
}
