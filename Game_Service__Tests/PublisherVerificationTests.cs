using Game_Service__Tests;
using Microsoft.AspNetCore.Identity;

namespace Game_Service__Tests;

public class PublisherVerificationTests : TestBase
{
    [Fact]
    public async Task BecomePublisher_SetsFlagsCorrectly()
    {
        var user = await CreateTestUser("newpub@test.com", "Test123!");
        var publisher = new GamePublisher { GamePublisherName = "Test Studio", CreatedAt = DateTime.UtcNow };
        DbContext.GamePublishers.Add(publisher);
        await DbContext.SaveChangesAsync();

        user.IsPublisherRepresentative = true;
        user.PublisherId = publisher.Id;
        user.PublisherRole = "Developer";
        user.IsPublisherVerified = false;
        await UserManager.UpdateAsync(user);

        var updated = await UserManager.FindByEmailAsync("newpub@test.com");
        Assert.True(updated!.IsPublisherRepresentative);
        Assert.False(updated.IsPublisherVerified);
        Assert.Equal(publisher.Id, updated.PublisherId);
    }

    [Fact]
    public async Task VerifyPublisher_SetsVerifiedFlag()
    {
        var user = await CreateTestUser("verify@test.com", "Test123!");
        var publisher = new GamePublisher { GamePublisherName = "Verify Studio", CreatedAt = DateTime.UtcNow };
        DbContext.GamePublishers.Add(publisher);
        await DbContext.SaveChangesAsync();

        user.IsPublisherRepresentative = true;
        user.PublisherId = publisher.Id;
        user.IsPublisherVerified = false;
        await UserManager.UpdateAsync(user);

        user.IsPublisherVerified = true;
        user.PublisherVerifiedAt = DateTime.UtcNow;
        await UserManager.UpdateAsync(user);

        var updated = await UserManager.FindByEmailAsync("verify@test.com");
        Assert.True(updated!.IsPublisherVerified);
        Assert.NotNull(updated.PublisherVerifiedAt);
    }

    [Fact]
    public async Task NonPublisherUser_HasNoPublisherAccess()
    {
        var user = await CreateTestUser("normal@test.com", "Test123!");
        Assert.False(user.IsPublisherRepresentative);
        Assert.Null(user.PublisherId);
        Assert.False(user.IsPublisherVerified);
    }

    [Fact]
    public async Task UnverifiedPublisher_IsNotVerified()
    {
        var user = await CreateTestUser("unverified@test.com", "Test123!");
        user.IsPublisherRepresentative = true;
        user.IsPublisherVerified = false;
        await UserManager.UpdateAsync(user);

        var updated = await UserManager.FindByEmailAsync("unverified@test.com");
        Assert.True(updated!.IsPublisherRepresentative);
        Assert.False(updated.IsPublisherVerified);
    }
}