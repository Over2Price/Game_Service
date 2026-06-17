using Game_Service__Tests;
using Microsoft.AspNetCore.Identity;

namespace Game_Service__Tests;

public class RegistrationTests : TestBase
{
    [Fact]
    public async Task Register_WithValidData_CreatesUser()
    {
        var user = new ApplicationUser
        {
            UserName = "newuser@test.com",
            Email = "newuser@test.com",
            CreatedAt = DateTime.UtcNow,
            Status = UserStatus.Active
        };
        var result = await UserManager.CreateAsync(user, "Test123!");
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task Register_WithShortPassword_Fails()
    {
        var user = new ApplicationUser { UserName = "short@test.com", Email = "short@test.com" };
        var result = await UserManager.CreateAsync(user, "Ab1!");
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsError()
    {
        await CreateTestUser("existing@test.com", "Test123!");
        var duplicate = new ApplicationUser { UserName = "existing@test.com", Email = "existing@test.com" };
        var result = await UserManager.CreateAsync(duplicate, "Test123!");
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Register_WithoutDigitInPassword_Fails()
    {
        var user = new ApplicationUser { UserName = "nodigit@test.com", Email = "nodigit@test.com" };
        var result = await UserManager.CreateAsync(user, "TestTest!");
        Assert.False(result.Succeeded);
    }

    [Fact]
    public async Task Register_AssignsUserRole_ByDefault()
    {
        var user = await CreateTestUser("roleuser@test.com", "Test123!");
        await UserManager.AddToRoleAsync(user, "User");
        Assert.True(await UserManager.IsInRoleAsync(user, "User"));
    }

    [Fact]
    public async Task Register_DoesNotAssignAdminRole_ByDefault()
    {
        var user = await CreateTestUser("noadmin@test.com", "Test123!");
        Assert.False(await UserManager.IsInRoleAsync(user, "Admin"));
    }
}