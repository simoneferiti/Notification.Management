using Moq;
using Notification.Core.Config;
using Notification.Core.Interface;
using Notification.Core.Models;
using Notification.Services;

namespace Notification.Test;

[TestFixture]
public class NotificationDispatcherTests
{
    private static Mock<INotificationChannel> CreateChannelMock(string name, bool shouldHandle = true)
    {
        var mock = new Mock<INotificationChannel>();
        mock.Setup(c => c.Name).Returns(name);
        mock.Setup(c => c.ShouldHandle(It.IsAny<NotificationEvent>())).Returns(shouldHandle);
        mock.Setup(c => c.DeliverAsync(It.IsAny<NotificationEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        return mock;
    }

    [Test]
    public async Task PublishAsync_ConsegnaSoloAiCanaliAbilitatiEChePossonoGestireLaNotifica()
    {
        var enabledChannel = CreateChannelMock("display");
        var disabledChannel = CreateChannelMock("email");
        var notHandlingChannel = CreateChannelMock("logFile", shouldHandle: false);

        var config = new ChannelConfig
        {
            Channels = new Dictionary<string, bool>
            {
                ["display"] = true,
                ["email"] = false,
                ["logFile"] = true
            }
        };

        var dispatcher = new NotificationDispatcher(
            new[] { enabledChannel.Object, disabledChannel.Object, notHandlingChannel.Object },
            config);

        var notification = NotificationEvent.Create("T", "M", NotificationPriority.Medium);
        await dispatcher.PublishAsync(notification);

        enabledChannel.Verify(c => c.DeliverAsync(notification, It.IsAny<CancellationToken>()), Times.Once);
        disabledChannel.Verify(c => c.DeliverAsync(It.IsAny<NotificationEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        notHandlingChannel.Verify(c => c.DeliverAsync(It.IsAny<NotificationEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Test]
    public async Task PublishAsync_AggiungeLaNotificaAllaHistoryEsollevaEvento()
    {
        var config = new ChannelConfig { Channels = new Dictionary<string, bool> { ["display"] = true } };
        var dispatcher = new NotificationDispatcher(new[] { CreateChannelMock("display").Object }, config);

        NotificationEvent? raised = null;
        dispatcher.NotificationPublished += (_, n) => raised = n;

        var notification = NotificationEvent.Create("T", "M", NotificationPriority.High);
        await dispatcher.PublishAsync(notification);

        Assert.That(dispatcher.History, Does.Contain(notification));
        Assert.That(raised, Is.EqualTo(notification));
    }

    [Test]
    public async Task PublishAsync_UnCanaleCheFalliscePermetteAgliAltriDiConsegnare()
    {
        var failingChannel = CreateChannelMock("email");
        failingChannel.Setup(c => c.DeliverAsync(It.IsAny<NotificationEvent>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("SMTP non raggiungibile"));
        var workingChannel = CreateChannelMock("display");

        var config = new ChannelConfig
        {
            Channels = new Dictionary<string, bool> { ["email"] = true, ["display"] = true }
        };
        var dispatcher = new NotificationDispatcher(new[] { failingChannel.Object, workingChannel.Object }, config);

        var notification = NotificationEvent.Create("T", "M", NotificationPriority.Critical);

        Assert.DoesNotThrowAsync(() => dispatcher.PublishAsync(notification));
        workingChannel.Verify(c => c.DeliverAsync(notification, It.IsAny<CancellationToken>()), Times.Once);
    }
}
