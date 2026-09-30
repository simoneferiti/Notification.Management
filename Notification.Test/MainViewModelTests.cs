using Notification.Core.Interface;
using Notification.Core.Models;
using NotificationApp.App.ViewModels;
using NotificationApp.Services;
using NUnit.Framework;

namespace NotificationApp.Tests;

/// <summary>Esegue subito l'azione, in modo sincrono: nessun vero thread UI necessario nei test.</summary>
file sealed class ImmediateUiDispatcher : IUiDispatcher
{
    public void Enqueue(Action action) => action();
}

/// <summary>Canale fittizio "sempre attivo" per non dipendere da implementazioni reali nei test del ViewModel.</summary>
file sealed class FakeChannel : INotificationChannel
{
    public string Name { get; }
    public FakeChannel(string name) => Name = name;
    public bool ShouldHandle(NotificationEvent notification) => true;
    public Task DeliverAsync(NotificationEvent notification, CancellationToken cancellationToken = default) => Task.CompletedTask;
}

[TestFixture]
public class MainViewModelTests
{
    private MainViewModel CreateViewModel()
    {
        var config = new ChannelConfig { Channels = new Dictionary<string, bool> { ["display"] = true } };
        var dispatcher = new NotificationDispatcher(new INotificationChannel[] { new FakeChannel("display") }, config);
        return new MainViewModel(dispatcher, new ImmediateUiDispatcher());
    }

    [Test]
    public async Task PublishCommand_AggiungeLaNotificaACorrentiEHistory()
    {
        var vm = CreateViewModel();
        vm.Title = "Titolo di test";
        vm.Message = "Messaggio di test";
        vm.SelectedPriority = NotificationPriority.Medium;

        await vm.PublishCommand.ExecuteAsync(null);

        Assert.That(vm.CurrentNotifications, Has.Count.EqualTo(1));
        Assert.That(vm.History, Has.Count.EqualTo(1));
        Assert.That(vm.CurrentNotifications[0].Title, Is.EqualTo("Titolo di test"));
    }

    [Test]
    public void PublishCommand_NonEseguibileSeIlTitoloEVuoto()
    {
        var vm = CreateViewModel();
        vm.Title = string.Empty;

        Assert.That(vm.PublishCommand.CanExecute(null), Is.False);
    }

    [Test]
    public async Task PublishCommand_SvuotaTitoloEMessaggioDopoLaPubblicazione()
    {
        var vm = CreateViewModel();
        vm.Title = "T";
        vm.Message = "M";

        await vm.PublishCommand.ExecuteAsync(null);

        Assert.That(vm.Title, Is.Empty);
        Assert.That(vm.Message, Is.Empty);
    }
}
