using Notification.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.UI.ViewModels;

public sealed class NotificationViewModel
{
    public NotificationViewModel(NotificationEvent notification)
    {
        Notification = notification;
    }

    public NotificationEvent Notification { get; }

    public string Title => Notification.Title;
    public string Message => Notification.Message;
    public string Priority => Notification.Priority.ToString();
    public string TimestampDisplay => Notification.Timestamp.ToString("dd/MM/yyyy HH:mm:ss");

    /// <summary>
    /// Nome della risorsa colore da usare in XAML (definita in App.xaml come
    /// StaticResource), es. "PriorityLowBrush", "PriorityCriticalBrush".
    /// Tenere la mappatura qui invece che nel code-behind della View.
    /// </summary>
    public string PriorityBrushKey => $"Priority{Notification.Priority}Brush";
}

internal class NotificationEventViewModel
{
}
