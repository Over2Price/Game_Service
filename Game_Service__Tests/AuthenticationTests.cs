using Game_Service__Tests;
using Microsoft.AspNetCore.Identity;

namespace Game_Service__Tests;

public class AuthenticationTests : TestBase
{
    [Fact]
    public async Task CheckPassword_WithCorrectPassword_ReturnsTrue()
    {
        var user = await CreateTestUser("login@test.com", "Test123!");
        Assert.True(await UserManager.CheckPasswordAsync(user, "Test123!"));
    }

    [Fact]
    public async Task CheckPassword_WithWrongPassword_ReturnsFalse()
    {
        var user = await CreateTestUser("wrong@test.com", "Test123!");
        Assert.False(await UserManager.CheckPasswordAsync(user, "WrongPass1!"));
    }

    [Fact]
    public async Task ConfirmEmail_WithValidToken_SetsEmailConfirmed()
    {
        var user = await CreateTestUser("confirm@test.com", "Test123!");
        var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        var result = await UserManager.ConfirmEmailAsync(user, token);
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task ConfirmEmail_WithInvalidToken_Fails()
    {
        var user = await CreateTestUser("invalid@test.com", "Test123!");
        var result = await UserManager.ConfirmEmailAsync(user, "invalid-token");
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task GenerateEmailToken_ReturnsNonNullToken()
    {
        var user = await CreateTestUser("token@test.com", "Test123!");
        var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task GenerateChangeEmailToken_ReturnsValidToken()
    {
        var user = await CreateTestUser("changeemail@test.com", "Test123!");
        var token = await UserManager.GenerateChangeEmailTokenAsync(user, "new@test.com");
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task IsEmailConfirmed_ReturnsFalse_ForNewUser()
    {
        var user = await CreateTestUser("new@test.com", "Test123!");
        Assert.False(await UserManager.IsEmailConfirmedAsync(user));
    }
}