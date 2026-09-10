using Core.Entities;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (!await db.Roles.AnyAsync()) { db.Roles.AddRange(new Role { Id = Guid.NewGuid(), Name = "ADMIN" }, new Role { Id = Guid.NewGuid(), Name = "USER" }); await db.SaveChangesAsync(); }
    }
}


