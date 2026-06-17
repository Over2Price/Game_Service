using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Game_Service.Services
{
    public class MailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool UseSSL { get; set; }
        public string From { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }

    public class MailData
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string? Body { get; set; }

        public MailData(string to, string subject, string? body = null)
        {
            To = to;
            Subject = subject;
            Body = body;
        }
    }

    public interface IMailService
    {
        Task<(bool success, string error)> SendAsync(MailData mailData);
    }

    public class MailService : IMailService
    {
        private readonly MailSettings _settings;
        private readonly ILogger<MailService> _logger;

        public MailService(IOptions<MailSettings> settings, ILogger<MailService> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task<(bool success, string error)> SendAsync(MailData mailData)
        {
            try
            {
                _logger.LogInformation("=== НАЧАЛО ОТПРАВКИ ===");
                _logger.LogInformation("Хост: {Host}:{Port}", _settings.Host, _settings.Port);
                _logger.LogInformation("SSL: {SSL}", _settings.UseSSL);
                _logger.LogInformation("От: {From}", _settings.From);
                _logger.LogInformation("Кому: {To}", mailData.To);
                _logger.LogInformation("Тема: {Subject}", mailData.Subject);
                _logger.LogInformation("Пароль (первые 3 символа): {Pwd}...",
                    _settings.Password?.Length > 3 ? _settings.Password[..3] : "пусто");

                var mail = new MimeMessage();
                mail.From.Add(new MailboxAddress(_settings.DisplayName, _settings.From));
                mail.To.Add(MailboxAddress.Parse(mailData.To));
                mail.Subject = mailData.Subject;
                mail.Body = new TextPart(TextFormat.Html) { Text = mailData.Body ?? "" };

                using var smtpClient = new SmtpClient();

                _logger.LogInformation("Подключение к серверу...");
                await smtpClient.ConnectAsync(_settings.Host, _settings.Port, _settings.UseSSL);
                _logger.LogInformation("Подключено!");

                smtpClient.AuthenticationMechanisms.Remove("XOAUTH2");

                _logger.LogInformation("Аутентификация...");
                await smtpClient.AuthenticateAsync(_settings.UserName, _settings.Password);
                _logger.LogInformation("Аутентификация успешна!");

                _logger.LogInformation("Отправка письма...");
                await smtpClient.SendAsync(mail);
                _logger.LogInformation("Письмо отправлено!");

                await smtpClient.DisconnectAsync(true);
                _logger.LogInformation("=== ОТПРАВКА ЗАВЕРШЕНА УСПЕШНО ===");

                return (true, "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "=== ОШИБКА ОТПРАВКИ ===");
                _logger.LogError("Тип ошибки: {Type}", ex.GetType().Name);
                _logger.LogError("Сообщение: {Message}", ex.Message);

                if (ex.InnerException != null)
                {
                    _logger.LogError("Внутренняя ошибка: {Inner}", ex.InnerException.Message);
                }

                return (false, $"{ex.GetType().Name}: {ex.Message}");
            }
        }
    }
}