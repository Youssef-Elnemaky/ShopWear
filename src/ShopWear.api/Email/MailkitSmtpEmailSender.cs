using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;
using ShopWear.Application.Services.Email;


public sealed class MailkitSmtpEmailSender : IEmailService
{
    private readonly SmtpOptions _options;
    public MailkitSmtpEmailSender(IOptions<SmtpOptions> options) => _options = options.Value;

    public async Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
    {
        var msg = new MimeMessage();
        msg.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
        msg.To.Add(MailboxAddress.Parse(toEmail));
        msg.Subject = subject;

        var body = new BodyBuilder { HtmlBody = htmlBody, TextBody = Regex.Replace(htmlBody ?? "", "<.*?>", "") };
        msg.Body = body.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_options.Host, _options.Port, _options.UseStartTls ? SecureSocketOptions.StartTlsWhenAvailable : SecureSocketOptions.Auto, ct);
        await smtp.AuthenticateAsync(_options.User, _options.Pass, ct);
        await smtp.SendAsync(msg, ct);
        await smtp.DisconnectAsync(true, ct);
    }
}