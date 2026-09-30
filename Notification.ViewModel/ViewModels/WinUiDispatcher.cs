using Microsoft.UI.Dispatching;
using System;

namespace Notification.ViewModels;

/// <summary>Implementazione reale: inoltra al DispatcherQueue della finestra WinUI.</summary>
public sealed class WinUiDispatcher : IUiDispatcher
{
    private readonly DispatcherQueue _dispatcherQueue;

    public WinUiDispatcher(DispatcherQueue dispatcherQueue)
    {
        _dispatcherQueue = dispatcherQueue;
    }

    public void Enqueue(Action action) => _dispatcherQueue.TryEnqueue(() => action());
}
