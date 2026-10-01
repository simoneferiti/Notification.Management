using Moq;
using Notification.Core.Models;
using NotificationApp.Services.Abstractions;
using NotificationApp.Services.Channels;

namespace Notification.Test;

[TestFixture]
public class EmailChannelTests
{
    private Mock<ISmtpClient> _smtpClientMock = null!;
    private EmailChannel _channel = null!;

    [SetUp]
    public void SetUp()
    {
        _smtpClientMock = new Mock<ISmtpClient>();
        _channel = new EmailChannel(_smtpClientMock.Object);
    }

    [TestCase(NotificationPriority.Low, false)]
    [TestCase(NotificationPriority.Medium, false)]
    [TestCase(NotificationPriority.High, true)]
    [TestCase(NotificationPriority.Critical, true)]
    public void ShouldHandle_RispettaIlFiltroDiPriorita(NotificationPriority priority, bool expected)
    {
        var notification = NotificationEvent.Create("Titolo", "Messaggio", priority);

        var result = _channel.ShouldHandle(notification);

        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public async Task DeliverAsync_InvocaLoSmtpClientConSubjectEBody()
    {
        var notification = NotificationEvent.Create("Errore critico", "Il servizio X non risponde", NotificationPriority.Critical);

        await _channel.DeliverAsync(notification);

        _smtpClientMock.Verify(
            s => s.SendAsync(
                It.Is<string>(subject => subject.Contains("Critical") && subject.Contains("Errore critico")),
                notification.Message,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
