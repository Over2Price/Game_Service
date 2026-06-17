using Game_Service;
using Game_Service.Data;
using Game_Service.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Game_Service_Tests.Tests;

public abstract class TestBase : IDisposable
{
    protected readonly ApplicationDbContext DbContext;
    protected readonly UserManager<ApplicationUser> UserManager;
    protected readonly RoleManager<IdentityRole> RoleManager;
    protected readonly SignInManager<ApplicationUser> SignInManager;

    protected TestBase()
    {
        var services = new ServiceCollection();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager();

        var provider = services.BuildServiceProvider();

        DbContext = provider.GetRequiredService<ApplicationDbContext>();
        UserManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        RoleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();
        SignInManager = provider.GetRequiredService<SignInManager<ApplicationUser>>();

        SeedRoles().GetAwaiter().GetResult();
    }

    private async Task SeedRoles()
    {
        foreach (var role in new[] { "Admin", "UserPublisher", "User" })
        {
            if (!await RoleManager.RoleExistsAsync(role))
                await RoleManager.CreateAsync(new IdentityRole(role));
        }
    }

    protected async Task<ApplicationUser> CreateTestUser(string email = "test@test.com",
        string password = "Test123!", bool confirmEmail = false)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = "Test User",
            CreatedAt = DateTime.UtcNow,
            Status = UserStatus.Active,
            EmailConfirmed = confirmEmail
        };

        var result = await UserManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        return user;
    }

    public void Dispose()
    {
        DbContext.Database.EnsureDeleted();
        DbContext.Dispose();
    }
}