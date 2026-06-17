using Game_Service__Tests;
using Microsoft.AspNetCore.Identity;

namespace Game_Service__Tests;

public class EmailChangeTests : TestBase
{
    [Fact]
    public async Task ChangeEmail_WithValidToken_Succeeds()
    {
        var user = await CreateTestUser("changer@test.com", "Test123!");
        var token = await UserManager.GenerateChangeEmailTokenAsync(user, "changed@test.com");
        var result = await UserManager.ChangeEmailAsync(user, "changed@test.com", token);
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task ChangeEmail_WithInvalidToken_Fails()
    {
        var user = await CreateTestUser("invalidch@test.com", "Test123!");
        var result = await UserManager.ChangeEmailAsync(user, "new@test.com", "bad-token");
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task ChangePassword_WithCorrectCurrentPassword_Succeeds()
    {
        var user = await CreateTestUser("passchange@test.com", "Test123!");
        var result = await UserManager.ChangePasswordAsync(user, "Test123!", "NewPass123!");
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task ChangePassword_WithWrongCurrentPassword_Fails()
    {
        var user = await CreateTestUser("wrongpass@test.com", "Test123!");
        var result = await UserManager.ChangePasswordAsync(user, "WrongOldPass1!", "NewPass123!");
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task ChangePassword_WithShortNewPassword_Fails()
    {
        var user = await CreateTestUser("shortpass@test.com", "Test123!");
        var result = await UserManager.ChangePasswordAsync(user, "Test123!", "Ab1!");
        Assert.False(result.Succeeded);
    }
}