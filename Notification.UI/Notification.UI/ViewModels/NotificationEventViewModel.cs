using Notification.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notification.UI.ViewModels;

public sealed class NotificationEventViewModel
{
    public NotificationEventViewModel(NotificationEvent notification)
    {
        Notification = notification;
    }

    public NotificationEvent Notification { get; }

    public string Title => Notification.Title;
    public string Message => Notification.Message;
    public string Priority => Notification.Priority.ToString();
    public string TimestampDisplay => Notification.Timestamp.ToString("dd/MM/yyyy HH:mm:ss");

    public string PriorityBrushKey => $"Priority{Notification.Priority}Brush";
}

