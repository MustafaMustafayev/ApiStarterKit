using ENTITIES.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.EntityFramework.Seeds;

public static class UserSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = new Guid("c3f4a1ee-8f34-4a6d-967c-d9e9e4356a47"), // Hardcoded to prevent changes
                CreatedAt = new DateTime(2024, 1, 1, 12, 0, 0), // Hardcoded to prevent changes
                Username = "Test",
                Email = "test@test.tst",
                ContactNumber = "+50000000",
                Password = "meFE2uI0CupBYDyyXWwolrdlvLOQsR7C3evDQJRl96nTDBRTV4kbkklomPBtiTGd/o2tjYbSR0yYGZyq/qOsDg==", // Hardcoded password: Test123@
                Salt = "9VzPlzXk9NuzNBP4V5TQWg=="
            }
        );
    }
}