using Game_Service.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Game_Service.Services
{
    public class IdentityEmailSender : IEmailSender<ApplicationUser>
    {
        private readonly IMailService _mailService;
        private readonly ILogger<IdentityEmailSender> _logger;

        public IdentityEmailSender(IMailService mailService, ILogger<IdentityEmailSender> logger)
        {
            _mailService = mailService;
            _logger = logger;
        }

        public async Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
        {
            var subject = "Подтверждение email — Game Service";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto;'>
                    <h2 style='color: #2563EB;'>Добро пожаловать в Game Service!</h2>
                    <p>Здравствуйте{(!string.IsNullOrEmpty(user.DisplayName) ? $", {user.DisplayName}" : "")}!</p>
                    <p>Ваш аккаунт почти создан. Для подтверждения email перейдите по ссылке:</p>
                    <p>
                        <a href='{confirmationLink}' 
                           style='display: inline-block; background-color: #2563EB; color: white; padding: 12px 24px; text-decoration: none; border-radius: 8px; font-weight: bold;'>
                            Подтвердить email
                        </a>
                    </p>
                    <p style='color: #64748B; font-size: 14px;'>Ссылка действительна 24 часа.</p>
                    <hr style='border: 1px solid #E2E8F0; margin: 20px 0;' />
                    <p style='color: #94A3B8; font-size: 12px;'>Если вы не регистрировались на Game Service, просто проигнорируйте это письмо.</p>
                </div>";

            var mailData = new MailData(email, subject, body);
            var (success, error) = await _mailService.SendAsync(mailData);

            if (success)
                _logger.LogInformation("Письмо подтверждения отправлено на {Email}", email);
            else
                _logger.LogWarning("Не удалось отправить письмо подтверждения на {Email}: {Error}", email, error);
        }

        public async Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
        {
            var subject = "Сброс пароля — Game Service";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto;'>
                    <h2 style='color: #2563EB;'>Сброс пароля</h2>
                    <p>Для сброса пароля перейдите по ссылке:</p>
                    <p>
                        <a href='{resetLink}' 
                           style='display: inline-block; background-color: #2563EB; color: white; padding: 12px 24px; text-decoration: none; border-radius: 8px; font-weight: bold;'>
                            Сбросить пароль
                        </a>
                    </p>
                    <p style='color: #64748B; font-size: 14px;'>Если вы не запрашивали сброс пароля, проигнорируйте это письмо.</p>
                </div>";

            var mailData = new MailData(email, subject, body);
            var (success, error) = await _mailService.SendAsync(mailData);

            if (success)
                _logger.LogInformation("Письмо сброса пароля отправлено на {Email}", email);
            else
                _logger.LogWarning("Не удалось отправить письмо сброса пароля на {Email}: {Error}", email, error);
        }

        public async Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
        {
            var subject = "Код сброса пароля — Game Service";
            var body = $@"
                <div style='font-family: Arial, sans-serif; max-width: 500px; margin: 0 auto;'>
                    <h2 style='color: #2563EB;'>Код сброса пароля</h2>
                    <p>Ваш код для сброса пароля:</p>
                    <p style='font-size: 32px; font-weight: bold; letter-spacing: 8px; color: #2563EB; text-align: center;'>{resetCode}</p>
                    <p style='color: #64748B; font-size: 14px;'>Код действителен 1 час.</p>
                </div>";

            var mailData = new MailData(email, subject, body);
            var (success, error) = await _mailService.SendAsync(mailData);

            if (success)
                _logger.LogInformation("Код сброса пароля отправлен на {Email}", email);
            else
                _logger.LogWarning("Не удалось отправить код сброса пароля на {Email}: {Error}", email, error);
        }
    }
}