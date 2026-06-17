using Game_Service__Tests;
using Microsoft.AspNetCore.Identity;

namespace Game_Service__Tests;

public class ProfileUpdateTests : TestBase
{
    [Fact]
    public async Task UpdateDisplayName_SavesCorrectly()
    {
        var user = await CreateTestUser("profile@test.com", "Test123!");
        user.DisplayName = "New Display Name";
        await UserManager.UpdateAsync(user);

        var updated = await UserManager.FindByEmailAsync("profile@test.com");
        Assert.Equal("New Display Name", updated!.DisplayName);
    }

    [Fact]
    public async Task UpdateBio_SavesCorrectly()
    {
        var user = await CreateTestUser("bio@test.com", "Test123!");
        user.Bio = "This is my bio";
        await UserManager.UpdateAsync(user);

        var updated = await UserManager.FindByEmailAsync("bio@test.com");
        Assert.Equal("This is my bio", updated!.Bio);
    }

    [Fact]
    public async Task UpdateCountryId_SavesCorrectly()
    {
        var country = new Country { Name = "Russia"};
        DbContext.Countries.Add(country);
        await DbContext.SaveChangesAsync();

        var user = await CreateTestUser("country@test.com", "Test123!");
        user.CountryId = country.Id;
        await UserManager.UpdateAsync(user);

        var updated = await UserManager.FindByEmailAsync("country@test.com");
        Assert.Equal(country.Id, updated!.CountryId);
    }

    [Fact]
    public async Task UpdateDateOfBirth_SavesCorrectly()
    {
        var user = await CreateTestUser("dob@test.com", "Test123!");
        var dob = new DateTime(2000, 5, 15);
        user.DateOfBirth = dob;
        await UserManager.UpdateAsync(user);

        var updated = await UserManager.FindByEmailAsync("dob@test.com");
        Assert.Equal(dob, updated!.DateOfBirth);
    }

    [Fact]
    public async Task UpdateAvatarUrl_SavesCorrectly()
    {
        var user = await CreateTestUser("avatar@test.com", "Test123!");
        user.AvatarUrl = "/uploads/avatars/test.png";
        await UserManager.UpdateAsync(user);

        var updated = await UserManager.FindByEmailAsync("avatar@test.com");
        Assert.Equal("/uploads/avatars/test.png", updated!.AvatarUrl);
    }
}