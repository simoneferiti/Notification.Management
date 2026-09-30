//using Notification.Core.Models;
//using Notification.Test;
//using NotificationApp.Services.Channels;
//using NUnit.Framework;


//namespace NotificationApp.Tests;

//[TestFixture]
//public class LogFileChannelTests
//{
//    [Test]
//    public async Task DeliverAsync_ScriveLaRigaNelFileDiLogSimulato()
//    {
//        var mockFileSystem = FakeFileWriter;
//        var channel = new LogFileChannel(mockFileSystem, "notifications.log");
//        var notification = NotificationEvent.Create("Backup completato", "Backup notturno OK", NotificationPriority.Low);

//        await channel.DeliverAsync(notification);

//        var content = mockFileSystem.File.ReadAllText("notifications.log");
//        Assert.That(content, Does.Contain("Backup completato"));
//        Assert.That(content, Does.Contain("Low"));
//    }

//    [Test]
//    public void ShouldHandle_GestisceSempreQualsiasiPriorita()
//    {
//        var channel = new LogFileChannel(new MockFileSystem());
//        var notification = NotificationEvent.Create("T", "M", NotificationPriority.Critical);

//        Assert.That(channel.ShouldHandle(notification), Is.True);
//    }
//}
