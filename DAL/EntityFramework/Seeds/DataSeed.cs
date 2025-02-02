using Microsoft.EntityFrameworkCore;

namespace DAL.EntityFramework.Seeds;

public static class DataSeed
{
    public static void Seed(ModelBuilder modelBuilder)
    {
        UserSeed.Seed(modelBuilder);
    }
}