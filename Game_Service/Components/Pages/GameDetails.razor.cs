using Game_Service.Data;
using Game_Service.Data.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Game_Service.Components.Pages;

public partial class GameDetails
{
    [Parameter]
    public int Id { get; set; }

    private bool isLoading = true;
    private bool isAuthenticated;
    private bool isEditing;
    private bool isSubmittingReview;
    private string? currentUserId;

    private Game? game;
    private GameReview? userReview;
    private List<GameReview> otherReviews = new();
    private ReviewModel reviewModel = new();

    private double? averageRating;
    private int totalReviews;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        isAuthenticated = user.Identity?.IsAuthenticated == true;

        if (isAuthenticated)
        {
            var appUser = await UserManager.GetUserAsync(user);
            currentUserId = appUser?.Id;
        }

        await LoadGame();
    }

    private async Task LoadGame()
    {
        isLoading = true;
        try
        {
            using var scope = ScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            game = await dbContext.Games
                .Include(g => g.Publisher)
                .Include(g => g.GameCategories)
                    .ThenInclude(gc => gc.Category)
                .FirstOrDefaultAsync(g => g.Id == Id);

            if (game != null)
            {
                var reviews = await dbContext.GameReviews
                    .Include(r => r.User)
                    .Where(r => r.GameId == Id && r.Status == ReviewStatus.Published)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                userReview = reviews.FirstOrDefault(r => r.UserId == currentUserId);
                otherReviews = reviews.Where(r => r.UserId != currentUserId).ToList();

                var allReviews = await dbContext.GameReviews
                .Where(r => r.GameId == Id && r.Status == ReviewStatus.Published)
                .ToListAsync();
                totalReviews = allReviews.Count;
                averageRating = allReviews.Any()
                    ? Math.Round(allReviews.Average(r => (double)r.Rating), 1)
                    : null;

                if (userReview != null)
                {
                    reviewModel = new ReviewModel
                    {
                        Rating = userReview.Rating,
                        Title = userReview.Title ?? string.Empty,
                        Content = userReview.Content ?? string.Empty,
                        IsRecommended = userReview.IsRecommended
                    };
                }
            }
        }
        finally
        {
            isLoading = false;
        }
    }

    private void StartEditing()
    {
        isEditing = true;
    }

    private void CancelEditing()
    {
        isEditing = false;
        if (userReview != null)
        {
            reviewModel = new ReviewModel
            {
                Rating = userReview.Rating,
                Title = userReview.Title ?? string.Empty,
                Content = userReview.Content ?? string.Empty,
                IsRecommended = userReview.IsRecommended
            };
        }
    }

    private async Task SubmitReview()
    {
        isSubmittingReview = true;
        try
        {
            using var scope = ScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (userReview != null)
            {
                // Обновление существующего
                var entity = await dbContext.GameReviews.FindAsync(userReview.Id);
                if (entity != null)
                {
                    entity.Rating = reviewModel.Rating;
                    entity.Title = reviewModel.Title?.Trim();
                    entity.Content = reviewModel.Content?.Trim();
                    entity.IsRecommended = reviewModel.IsRecommended;
                    entity.UpdatedAt = DateTime.UtcNow;
                    await dbContext.SaveChangesAsync();
                }
            }
            else if (currentUserId != null)
            {
                // Новый отзыв
                var newReview = new GameReview
                {
                    UserId = currentUserId,
                    GameId = Id,
                    Rating = reviewModel.Rating,
                    Title = reviewModel.Title?.Trim(),
                    Content = reviewModel.Content?.Trim(),
                    IsRecommended = reviewModel.IsRecommended,
                    CreatedAt = DateTime.UtcNow
                };
                dbContext.GameReviews.Add(newReview);
                await dbContext.SaveChangesAsync();
            }

            isEditing = false;
            await LoadGame();
        }
        finally
        {
            isSubmittingReview = false;
        }
    }

    private async Task DeleteReview()
    {
        if (userReview == null) return;

        using var scope = ScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var entity = await dbContext.GameReviews.FindAsync(userReview.Id);
        if (entity != null)
        {
            dbContext.GameReviews.Remove(entity);
            await dbContext.SaveChangesAsync();
        }
        await LoadGame();
    }

    private string? GetYouTubeEmbedUrl(string? url)
    {
        if (string.IsNullOrEmpty(url)) return null;

        // Извлекаем ID видео
        string? videoId = null;

        // https://www.youtube.com/watch?v=XXXXX
        if (url.Contains("youtube.com/watch"))
        {
            var uri = new Uri(url);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            videoId = query["v"];
        }
        // https://youtu.be/XXXXX
        else if (url.Contains("youtu.be"))
        {
            videoId = url.Split('/').Last().Split('?').First();
        }

        return videoId != null ? $"https://www.youtube.com/embed/{videoId}" : null;
    }

    private class ReviewModel
    {
        [Required, Range(1, 10)]
        public int Rating { get; set; } = 8;

        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(5000)]
        public string Content { get; set; } = string.Empty;

        public bool IsRecommended { get; set; } = true;
    }
}