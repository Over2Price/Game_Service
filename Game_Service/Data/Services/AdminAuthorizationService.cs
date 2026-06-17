using Game_Service.Data.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Game_Service.Services
{
    /// <summary>
    /// Сервис для проверки прав администратора.
    /// Использовать во всех действиях админ-панели как дополнительный уровень защиты.
    /// </summary>
    public class AdminAuthorizationService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminAuthorizationService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Проверяет, является ли пользователь администратором.
        /// Вызывается перед выполнением любых админ-действий.
        /// </summary>
        public async Task<bool> IsAdminAsync(ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated != true)
                return false;

            var applicationUser = await _userManager.GetUserAsync(user);
            if (applicationUser == null)
                return false;

            return await _userManager.IsInRoleAsync(applicationUser, "Admin");
        }

        /// <summary>
        /// Проверяет права и бросает исключение, если не админ.
        /// Использовать в методах API/сервисов, где нет атрибута [Authorize].
        /// </summary>
        public async Task EnsureAdminAsync(ClaimsPrincipal user)
        {
            if (!await IsAdminAsync(user))
                throw new UnauthorizedAccessException("Требуются права администратора.");
        }
    }
}