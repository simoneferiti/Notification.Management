using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Notification.Core.Interface;
using Notification.Core.Models;
using NotificationApp.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.UI.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly INotificationDispatcher _dispatcher;
    private readonly IUiDispatcher _uiDispatcher;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PublishCommand))]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _message = string.Empty;

    [ObservableProperty]
    private NotificationPriority _selectedPriority = NotificationPriority.Low;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PublishCommand))]
    private bool _isPublishing;

    public ObservableCollection<NotificationEventViewModel> CurrentNotifications { get; } = new();
    public ObservableCollection<NotificationEventViewModel> History { get; } = new();

    public MainViewModel(INotificationDispatcher dispatcher, IUiDispatcher uiDispatcher)
    {
        _dispatcher = dispatcher;
        _uiDispatcher = uiDispatcher;
        _dispatcher.NotificationPublished += OnNotificationPublished;
    }

    [RelayCommand(CanExecute = nameof(CanPublish))]
    private async Task PublishAsync()
    {
        IsPublishing = true;
        try
        {
            var notification = NotificationEvent.Create(Title, Message, SelectedPriority);
            await _dispatcher.PublishAsync(notification);
            Title = string.Empty;
            Message = string.Empty;
        }
        finally
        {
            IsPublishing = false;
        }
    }

    private bool CanPublish() => !IsPublishing && !string.IsNullOrWhiteSpace(Title);

    /// <summary>
    /// L'evento del dispatcher può arrivare da un thread di background (i canali
    /// girano in parallelo con Task.WhenAll): ogni aggiornamento delle
    /// ObservableCollection deve essere marshalled sul thread UI (tramite
    /// IUiDispatcher) per non violare il threading model di WinUI. Nei test
    /// si inietta un IUiDispatcher che esegue subito, in modo sincrono.
    /// </summary>
    private void OnNotificationPublished(object? sender, NotificationEvent notification)
    {
        _uiDispatcher.Enqueue(() =>
        {
            var vm = new NotificationEventViewModel(notification);
            CurrentNotifications.Insert(0, vm);
            History.Insert(0, vm);
        });
    }
}


