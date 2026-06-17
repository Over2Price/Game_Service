using Game_Service__Tests;
using Microsoft.AspNetCore.Identity;

namespace Game_Service__Tests;

public class AuthorizationAndRolesTests : TestBase
{
    [Fact]
    public async Task AdminRole_Exists() =>
        Assert.True(await RoleManager.RoleExistsAsync("Admin"));

    [Fact]
    public async Task UserPublisherRole_Exists() =>
        Assert.True(await RoleManager.RoleExistsAsync("UserPublisher"));

    [Fact]
    public async Task UserRole_Exists() =>
        Assert.True(await RoleManager.RoleExistsAsync("User"));

    [Fact]
    public async Task AssignAdminRole_MakesUserAdmin()
    {
        var user = await CreateTestUser("admin@test.com", "Test123!");
        await UserManager.AddToRoleAsync(user, "Admin");
        Assert.True(await UserManager.IsInRoleAsync(user, "Admin"));
    }

    [Fact]
    public async Task AssignUserPublisherRole_MakesUserPublisher()
    {
        var user = await CreateTestUser("pub@test.com", "Test123!");
        await UserManager.AddToRoleAsync(user, "UserPublisher");
        Assert.True(await UserManager.IsInRoleAsync(user, "UserPublisher"));
    }

    [Fact]
    public async Task DefaultUser_DoesNotHaveAdminRole()
    {
        var user = await CreateTestUser("normal@test.com", "Test123!");
        Assert.False(await UserManager.IsInRoleAsync(user, "Admin"));
    }

    [Fact]
    public async Task RemoveRole_RevokesAccess()
    {
        var user = await CreateTestUser("remove@test.com", "Test123!");
        await UserManager.AddToRoleAsync(user, "UserPublisher");
        await UserManager.RemoveFromRoleAsync(user, "UserPublisher");
        Assert.False(await UserManager.IsInRoleAsync(user, "UserPublisher"));
    }
}