using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Notification.Core.Interface;
using Notification.Service;
using Notification.UI.ViewModels;
using NotificationApp.Core.Interfaces;
using NotificationApp.Services;
using NotificationApp.Services.Abstractions;
using NotificationApp.Services.Channels;
using NotificationApp.Services.Config;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Notification.UI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;
        private IServiceProvider? _services;


        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            var services = new ServiceCollection();

            // Infrastruttura condivisa
            services.AddSingleton<IFileWriter, FileWriter>();
            services.AddSingleton<ISmtpClient, StubSmtpClient>();
            services.AddSingleton<IChannelConfigLoader, JsonChannelConfigLoader>();

            // Canali: registrati come INotificationChannel, il dispatcher li riceve tutti insieme
            services.AddSingleton<INotificationChannel, DisplayChannel>();
            services.AddSingleton<INotificationChannel, EmailChannel>();
            services.AddSingleton<INotificationChannel, LogFileChannel>();

            services.AddSingleton<INotificationDispatcher, NotificationDispatcher>();

            // ViewModel/Window
            services.AddSingleton(DispatcherQueue.GetForCurrentThread());
            services.AddSingleton<IUiDispatcher, WinUiDispatcher>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();

            _services = services.BuildServiceProvider();

            // La configurazione va caricata prima di creare il dispatcher: registriamo
            // ChannelConfig come istanza già risolta, non come servizio lazy, perché
            // il caricamento è asincrono e va fatto una sola volta all'avvio.
            var configLoader = _services.GetRequiredService<IChannelConfigLoader>();
            var channelConfig = await configLoader.LoadAsync();

            var servicesWithConfig = new ServiceCollection();
            foreach (var descriptor in services)
                servicesWithConfig.Add(descriptor);
            servicesWithConfig.AddSingleton(channelConfig);

            _services = servicesWithConfig.BuildServiceProvider();

            _window = _services.GetRequiredService<MainWindow>();
            _window.Activate();
        }
    }
}
