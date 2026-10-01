using Notification.Core.Models;
using Notification.Test.Fake;
using NotificationApp.Services.Channels;


namespace Notification.Test;

[TestFixture]
public class LogFileChannelTests
{
    [Test]
    public async Task DeliverAsync_ScriveLaRigaNelFileDiLogSimulato()
    {
        var fakeWriter = new FakeFileWriter();
        var channel = new LogFileChannel(fakeWriter, "notifications.log");
        var notification = NotificationEvent.Create("Backup completato", "Backup notturno OK", NotificationPriority.Low);

        await channel.DeliverAsync(notification);

        var content = fakeWriter.ReadAllTextAsync("notifications.log");
        Assert.That(await content, Does.Contain("Backup completato"));
        Assert.That(await content, Does.Contain("Low"));
    }

    [Test]
    public void ShouldHandle_GestisceSempreQualsiasiPriorita()
    {
        var channel = new LogFileChannel(new FakeFileWriter());
        var notification = NotificationEvent.Create("T", "M", NotificationPriority.Critical);

        Assert.That(channel.ShouldHandle(notification), Is.True);
    }
}
