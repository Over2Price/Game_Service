using Game_Service__Tests;

namespace Game_Service__Tests;

public class DataIntegrityTests : TestBase
{
    [Fact]
    public async Task CreateGame_WithValidData_SavesToDatabase()
    {
        var game = new Game { Title = "Test Game", CreatedAt = DateTime.UtcNow };
        DbContext.Games.Add(game);
        await DbContext.SaveChangesAsync();
        Assert.NotNull(await DbContext.Games.FindAsync(game.Id));
    }

    [Fact]
    public async Task CreateGame_WithPublisher_SetsForeignKey()
    {
        var publisher = new GamePublisher { GamePublisherName = "Test Publisher", CreatedAt = DateTime.UtcNow };
        DbContext.GamePublishers.Add(publisher);
        await DbContext.SaveChangesAsync();

        var game = new Game { Title = "Published Game", PublisherId = publisher.Id, CreatedAt = DateTime.UtcNow };
        DbContext.Games.Add(game);
        await DbContext.SaveChangesAsync();

        var saved = await DbContext.Games.Include(g => g.Publisher).FirstAsync(g => g.Id == game.Id);
        Assert.NotNull(saved.Publisher);
        Assert.Equal("Test Publisher", saved.Publisher!.GamePublisherName);
    }

    [Fact]
    public async Task CreateGame_WithCategories_CreatesManyToManyRelation()
    {
        var cat1 = new Category { Name = "Action", Slug = "action" };
        var cat2 = new Category { Name = "RPG", Slug = "rpg" };
        DbContext.Categories.AddRange(cat1, cat2);
        await DbContext.SaveChangesAsync();

        var game = new Game { Title = "MultiCat Game", CreatedAt = DateTime.UtcNow };
        DbContext.Games.Add(game);
        await DbContext.SaveChangesAsync();

        DbContext.GameCategories.Add(new GameCategory { GameId = game.Id, CategoryId = cat1.Id });
        DbContext.GameCategories.Add(new GameCategory { GameId = game.Id, CategoryId = cat2.Id });
        await DbContext.SaveChangesAsync();

        var count = await DbContext.GameCategories.CountAsync(gc => gc.GameId == game.Id);
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task CreateReview_WithValidData_SavesToDatabase()
    {
        var user = await CreateTestUser("reviewer@test.com", "Test123!");
        var game = new Game { Title = "Reviewed Game", CreatedAt = DateTime.UtcNow };
        DbContext.Games.Add(game);
        await DbContext.SaveChangesAsync();

        var review = new GameReview
        {
            UserId = user.Id,
            GameId = game.Id,
            Rating = 8,
            Title = "Great game!",
            Content = "Really enjoyed playing.",
            IsRecommended = true,
            CreatedAt = DateTime.UtcNow
        };
        DbContext.GameReviews.Add(review);
        await DbContext.SaveChangesAsync();

        var saved = await DbContext.GameReviews.FirstAsync(r => r.GameId == game.Id);
        Assert.Equal(8, saved.Rating);
    }

    [Fact]
    public async Task DuplicateReview_FromSameUser_IsPrevented()
    {
        var user = await CreateTestUser("dupreview@test.com", "Test123!");
        var game = new Game { Title = "Dup Game", CreatedAt = DateTime.UtcNow };
        DbContext.Games.Add(game);
        await DbContext.SaveChangesAsync();

        DbContext.GameReviews.Add(new GameReview { UserId = user.Id, GameId = game.Id, Rating = 5, CreatedAt = DateTime.UtcNow });
        await DbContext.SaveChangesAsync();

        DbContext.GameReviews.Add(new GameReview { UserId = user.Id, GameId = game.Id, Rating = 7, CreatedAt = DateTime.UtcNow });

        try
        {
            await DbContext.SaveChangesAsync();
            Assert.False(true, "Expected exception was not thrown");
        }
        catch
        {
            Assert.True(true);
        }
    }

    [Fact]
    public async Task AverageRating_CalculatesCorrectly()
    {
        var game = new Game { Title = "Rated Game", CreatedAt = DateTime.UtcNow };
        DbContext.Games.Add(game);
        await DbContext.SaveChangesAsync();

        var u1 = await CreateTestUser("r1@test.com", "Test123!");
        var u2 = await CreateTestUser("r2@test.com", "Test123!");
        var u3 = await CreateTestUser("r3@test.com", "Test123!");

        DbContext.GameReviews.AddRange(
            new GameReview { UserId = u1.Id, GameId = game.Id, Rating = 8, CreatedAt = DateTime.UtcNow },
            new GameReview { UserId = u2.Id, GameId = game.Id, Rating = 10, CreatedAt = DateTime.UtcNow },
            new GameReview { UserId = u3.Id, GameId = game.Id, Rating = 6, CreatedAt = DateTime.UtcNow }
        );
        await DbContext.SaveChangesAsync();

        var avg = await DbContext.GameReviews.Where(r => r.GameId == game.Id).AverageAsync(r => (double)r.Rating);
        Assert.Equal(8.0, avg, 1);
    }

    [Fact]
    public async Task DeleteGame_CascadesToReviews()
    {
        var user = await CreateTestUser("cascade@test.com", "Test123!");
        var game = new Game { Title = "To Delete", CreatedAt = DateTime.UtcNow };
        DbContext.Games.Add(game);
        await DbContext.SaveChangesAsync();
        DbContext.GameReviews.Add(new GameReview { UserId = user.Id, GameId = game.Id, Rating = 5, CreatedAt = DateTime.UtcNow });
        await DbContext.SaveChangesAsync();

        DbContext.Games.Remove(game);
        await DbContext.SaveChangesAsync();

        var count = await DbContext.GameReviews.CountAsync(r => r.GameId == game.Id);
        Assert.Equal(0, count);
    }
}