using CSharpClickerWeb.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CSharpClickerWeb.Initializers;

public static class DbContextInitializer
{
    public static void InitializeDbContext(IServiceCollection services)
    {
        var pathToDbFile = GetPathToDbFile();
        services
            .AddDbContext<AppDbContext>(options => options
                .UseSqlite($"Data Source={pathToDbFile}"));

        using var serviceProvider = services.BuildServiceProvider();
        var appDbContext = serviceProvider.GetRequiredService<AppDbContext>();

        appDbContext.Database.Migrate();

        string GetPathToDbFile()
        {
            var applicationFolder = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData), "CSharpClicker");

            if (!Directory.Exists(applicationFolder))
            {
                Directory.CreateDirectory(applicationFolder);
            }

            return Path.Combine(applicationFolder, "CSharpClicker.db");
        }
    }
}