using Game_Service.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace Game_Service.Services
{
    /// <summary>
    /// Сервис для создания ролей при запуске приложения.
    /// Вызывается один раз в Program.cs.
    /// </summary>
    public class RoleSeedService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RoleSeedService> _logger;

        public RoleSeedService(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<RoleSeedService> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Создаёт роли, если они ещё не существуют.
        /// </summary>
        public async Task SeedRolesAsync()
        {
            var roles = new[] { "Admin", "UserPublisher", "User" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(role));
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Роль {Role} успешно создана", role);
                    }
                    else
                    {
                        _logger.LogError("Ошибка при создании роли {Role}: {Errors}",
                            role, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
                else
                {
                    _logger.LogInformation("Роль {Role} уже существует", role);
                }
            }
        }

        /// <summary>
        /// Назначает роль Admin указанному пользователю по email.
        /// Вызывается вручную или из консоли администратора.
        /// </summary>
        public async Task AssignAdminRoleAsync(string adminEmail)
        {
            var user = await _userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                _logger.LogWarning("Пользователь с email {Email} не найден", adminEmail);
                return;
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                _logger.LogInformation("Пользователь {Email} уже является администратором", adminEmail);
                return;
            }

            var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (result.Succeeded)
            {
                _logger.LogInformation("Пользователь {Email} назначен администратором", adminEmail);
            }
            else
            {
                _logger.LogError("Ошибка при назначении Admin для {Email}: {Errors}",
                    adminEmail, string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}