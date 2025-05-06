using Microsoft.EntityFrameworkCore;

namespace TaskSystem.DataAccessLayer;

public static class DbInitializer
{
    public static void Initialize(ServiceDbContext context)
    {
        context.Database.Migrate();
    }
}