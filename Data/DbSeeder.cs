using Microsoft.EntityFrameworkCore;
using UserManagementApi.Models;
using UserManagementApi.Services;

namespace UserManagementApi.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext dbContext, IPasswordService passwordService)
    {
        if (await dbContext.Users.AnyAsync())
        {
            return;
        }

        var admin = new User
        {
            Name = "Admin User",
            Email = "admin@example.com",
            CreatedAt = DateTime.UtcNow
        };

        admin.Password = passwordService.HashPassword(admin, "Admin@123");

        dbContext.Users.Add(admin);
        await dbContext.SaveChangesAsync();
    }
}
