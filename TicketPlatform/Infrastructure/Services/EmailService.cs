using Application.IServices;
using Infrastructure.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptionsSnapshot<EmailOptions> _mailOptions;

        public EmailService(IOptionsSnapshot<EmailOptions> mailOptions)
        {
            _mailOptions = mailOptions;
        }
        public async Task SendEmailAsync(MimeMessage mailMessage, CancellationToken cancellationToken)
        {
            string host = _mailOptions.Value.EmailHost;
            int port = _mailOptions.Value.EmailPort;
            string userName = _mailOptions.Value.EmailUsername;
            string emailPassword = _mailOptions.Value.EmailPassword;
            var socketOptions = port switch
            {
                465 => SecureSocketOptions.SslOnConnect, 
                587 => SecureSocketOptions.StartTls,   
                25 => SecureSocketOptions.StartTlsWhenAvailable,
                _ => SecureSocketOptions.Auto
            };
            SmtpClient  smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(host, port, socketOptions, cancellationToken);
            await smtpClient.AuthenticateAsync(userName, emailPassword, cancellationToken);
            await smtpClient.SendAsync(mailMessage, cancellationToken);
            await smtpClient.DisconnectAsync(true, cancellationToken);
        }
        private MimeMessage CreateMimeMessage(string to, string subject, string body)
        {
            string userName = _mailOptions.Value.EmailUsername;
            BodyBuilder bodyBuilder = new BodyBuilder() { HtmlBody = body };
            MimeMessage mimeMessage = new MimeMessage()
            {
                Body = bodyBuilder.ToMessageBody(),
                Subject = subject,
            };
            mimeMessage.To.Add(MailboxAddress.Parse(to));
            mimeMessage.From.Add(new MailboxAddress("Auth Demo", userName));

            return mimeMessage;
        }
        public async Task SendResetPasswordEmailAsync(string to, string confirmationLink, CancellationToken cancellationToken)
        {
            string body = $@"
            <h2>Welcome to Freelancer Platform!</h2>
            <p>Please reset your account Password by clicking the link below:</p>
            <p><a href='{confirmationLink}'>Click here to reset your password</a></p>";
            string subject = "Reset your password address";
            MimeMessage mailMessage = CreateMimeMessage(to, subject, body);
            await SendEmailAsync(mailMessage, cancellationToken);
        }
        public async Task SendConfirmEmailAsync(string to, string confirmationLink, CancellationToken cancellationToken)
        {
            string body = $@"
            <h2>Welcome to Freelancer Platform!</h2>
            <p>Please confirm your account by clicking the link below:</p>
            <p><a href='{confirmationLink}'>Click here to confirm your email</a></p>";
            string subject = "Confirm your email address";
            MimeMessage mailMessage = CreateMimeMessage(to, subject, body);
            await SendEmailAsync(mailMessage, cancellationToken);
        }

        public async Task SendWarningEmailThatRefreshTokenStealAsync(string to, CancellationToken cancellationToken)
        {
            string body = $@"warning your refresh token has been stolen.
                             Reset your password immediately.";
            string subject = "Warning: Refresh Token Theft";
            MimeMessage mailMessage = CreateMimeMessage(to, subject, body);
            await SendEmailAsync(mailMessage, cancellationToken);
        }

    }
}
