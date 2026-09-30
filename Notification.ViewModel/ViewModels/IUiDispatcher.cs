using System;

namespace Notification.ViewModels;

/// <summary>
/// Astrazione minima sopra Microsoft.UI.Dispatching.DispatcherQueue.
/// Permette di testare MainViewModel senza dover avviare un vero thread UI WinUI:
/// nei test si inietta un'implementazione che esegue l'azione in modo sincrono/immediato.
/// </summary>
public interface IUiDispatcher
{
    void Enqueue(Action action);
}
