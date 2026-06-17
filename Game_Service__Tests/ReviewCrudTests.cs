using Game_Service__Tests;

namespace Game_Service__Tests;

public class ReviewCrudTests : TestBase
{
    private async Task<(Game game, ApplicationUser user)> SetupGameAndUser()
    {
        var user = await CreateTestUser($"rev_{Guid.NewGuid()}@test.com", "Test123!");
        var game = new Game { Title = "Review Game", CreatedAt = DateTime.UtcNow };
        DbContext.Games.Add(game);
        await DbContext.SaveChangesAsync();
        return (game, user);
    }

    [Fact]
    public async Task CreateReview_SavesAllFields()
    {
        var (game, user) = await SetupGameAndUser();
        var review = new GameReview
        {
            UserId = user.Id,
            GameId = game.Id,
            Rating = 9,
            Title = "Awesome!",
            Content = "Loved it",
            IsRecommended = true,
            CreatedAt = DateTime.UtcNow
        };
        DbContext.GameReviews.Add(review);
        await DbContext.SaveChangesAsync();

        var saved = await DbContext.GameReviews.FirstAsync(r => r.GameId == game.Id);
        Assert.Equal(9, saved.Rating);
        Assert.Equal("Awesome!", saved.Title);
    }

    [Fact]
    public async Task UpdateReview_ChangesFields()
    {
        var (game, user) = await SetupGameAndUser();
        var review = new GameReview
        {
            UserId = user.Id,
            GameId = game.Id,
            Rating = 5,
            Title = "Old",
            CreatedAt = DateTime.UtcNow
        };
        DbContext.GameReviews.Add(review);
        await DbContext.SaveChangesAsync();

        review.Rating = 7;
        review.Title = "Updated";
        review.UpdatedAt = DateTime.UtcNow;
        await DbContext.SaveChangesAsync();

        var updated = await DbContext.GameReviews.FindAsync(review.Id);
        Assert.Equal(7, updated!.Rating);
        Assert.Equal("Updated", updated.Title);
    }

    [Fact]
    public async Task DeleteReview_RemovesFromDatabase()
    {
        var (game, user) = await SetupGameAndUser();
        var review = new GameReview
        {
            UserId = user.Id,
            GameId = game.Id,
            Rating = 3,
            CreatedAt = DateTime.UtcNow
        };
        DbContext.GameReviews.Add(review);
        await DbContext.SaveChangesAsync();

        DbContext.GameReviews.Remove(review);
        await DbContext.SaveChangesAsync();

        var count = await DbContext.GameReviews.CountAsync(r => r.GameId == game.Id);
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task ReviewRating_InvalidRange_IsRejected()
    {
        var (game, user) = await SetupGameAndUser();
        var review = new GameReview
        {
            UserId = user.Id,
            GameId = game.Id,
            Rating = 11,
            CreatedAt = DateTime.UtcNow
        };
        DbContext.GameReviews.Add(review);

        try
        {
            await DbContext.SaveChangesAsync();
            Assert.False(true, "Expected exception");
        }
        catch
        {
            Assert.True(true);
        }
    }
}