namespace NotificationApp.Services.Abstractions;

/// <summary>
/// Abstraction minima sopra il client SMTP reale, per poter fare mock nei test
/// senza inviare email vere. In produzione l'implementazione userebbe
/// System.Net.Mail.SmtpClient o un provider esterno (SendGrid, ecc.).
/// </summary>
public interface ISmtpClient
{
    Task SendAsync(string subject, string body, CancellationToken cancellationToken = default);
}

/// <summary>
/// Stub di produzione: per l'assessment è sufficiente loggare l'invio.
/// Sostituire con un'implementazione reale quando serve.
/// </summary>
public sealed class StubSmtpClient : ISmtpClient
{
    public Task SendAsync(string subject, string body, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"[EMAIL STUB] {subject}: {body}");
        return Task.CompletedTask;
    }
}
