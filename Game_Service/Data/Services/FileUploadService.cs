using Microsoft.AspNetCore.Components.Forms;

namespace Game_Service.Services
{
    /// <summary>
    /// Сервис для сохранения загруженных файлов (аватарки пользователей).
    /// </summary>
    public class FileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileUploadService> _logger;
        private readonly string _avatarsFolder;

        public FileUploadService(IWebHostEnvironment environment, ILogger<FileUploadService> logger)
        {
            _environment = environment;
            _logger = logger;
            _avatarsFolder = Path.Combine(_environment.WebRootPath, "uploads", "avatars");
        }

        /// <summary>
        /// Сохраняет аватарку и возвращает относительный путь к файлу.
        /// </summary>
        public async Task<string?> SaveAvatarAsync(IBrowserFile file, string userId)
        {
            try
            {
                if (file == null || file.Size == 0)
                    return null;

                // Проверяем тип файла
                var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
                if (!allowedTypes.Contains(file.ContentType))
                {
                    _logger.LogWarning("Недопустимый тип файла: {ContentType}", file.ContentType);
                    return null;
                }

                // Проверяем размер (макс 2 МБ)
                if (file.Size > 2 * 1024 * 1024)
                {
                    _logger.LogWarning("Файл слишком большой: {Size} байт", file.Size);
                    return null;
                }

                // Создаём папку, если её нет
                if (!Directory.Exists(_avatarsFolder))
                    Directory.CreateDirectory(_avatarsFolder);

                // Генерируем уникальное имя файла
                var extension = Path.GetExtension(file.Name).ToLowerInvariant();
                var fileName = $"{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extension}";
                var filePath = Path.Combine(_avatarsFolder, fileName);

                // Сохраняем файл
                await using var stream = new FileStream(filePath, FileMode.Create);
                await file.OpenReadStream(maxAllowedSize: 2 * 1024 * 1024).CopyToAsync(stream);

                var relativePath = $"/uploads/avatars/{fileName}";
                _logger.LogInformation("Аватарка сохранена: {Path}", relativePath);

                return relativePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при сохранении аватарки");
                return null;
            }
        }
    }
}