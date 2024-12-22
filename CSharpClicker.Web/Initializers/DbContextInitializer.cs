using CSharpClicker.Web.Domain;
using CSharpClicker.Web.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace CSharpClicker.Web.Initializers;

public static class DbContextInitializer
{
    public static void AddAppDbContext(IServiceCollection services)
    {
        var pathToDbFile = GetPathToDbFile();
        services
            .AddDbContext<AppDbContext>(options => options
                .UseSqlite($"Data Source={pathToDbFile}"));

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

    public static void InitializeDbContext(AppDbContext appDbContext)
    {
        const string Boost1 = "Добавить андроида на автокарьерный шатл";
        const string Boost2 = "Добавить ПЭТН на автокарьерный шатл";
        const string Boost3 = "Малый плазменный добыватель руды";
        const string Boost4 = "Добавить ядерную взрывчетку на автокарьерный шатл";
        const string Boost5 = "Плазменный добыватель руды";

        appDbContext.Database.Migrate();

        var existingBoosts = appDbContext.Boosts
            .ToArray();

        AddBoostIfNotExist(Boost1, price: 100, profit: 1);
        AddBoostIfNotExist(Boost2, price: 500, profit: 15);
        AddBoostIfNotExist(Boost3, price: 1000, profit: 60, isAuto: true);
        AddBoostIfNotExist(Boost4, price: 3000, profit: 300);
        AddBoostIfNotExist(Boost5, price: 5000, profit: 600, isAuto: true);

        appDbContext.SaveChanges();

        void AddBoostIfNotExist(string name, long price, long profit, bool isAuto = false)
        {
            if (!existingBoosts.Any(eb => eb.Title == name))
            {
                var pathToImage = Path.Combine(".", "Resources", "BoostImages", $"{name}.png");
                using var fileStream = File.OpenRead(pathToImage);
                using var memoryStream = new MemoryStream();

                fileStream.CopyTo(memoryStream);

                appDbContext.Boosts.Add(new Boost
                {
                    Title = name,
                    Price = price,
                    Profit = profit,
                    IsAuto = isAuto,
                    Image = memoryStream.ToArray(),
                });
            }
        }
    }
}
