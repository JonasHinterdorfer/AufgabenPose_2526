using System.Threading.Tasks;

using Base.Tools.CsvImport;

using Core.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence;

using Xunit;

namespace UnitTest.Repository;

public class RepositoryTestFixture : RepositoryTestFixtureBase<ApplicationDbContext>, IAsyncLifetime
{
    public async ValueTask InitializeAsync()
    {
        //drop and recreate the test Db every time. 

        await InitializeDatabase();
    }

    public async ValueTask DisposeAsync()
    {
        await Task.CompletedTask;
    }

    private async Task InitializeDatabase()
    {
        using (var dbContext = CreateDbContext())
        {
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.MigrateAsync();

            await ImportDataAsync(dbContext);

            await dbContext.SaveChangesAsync();
        }
    }


    private async Task ImportDataAsync(ApplicationDbContext dbContext)
    {
        var competitions = new CsvImport<Competition>().Read(
            new[]
            {
                "Name",
                "Competition 1",
                "Competition 2",
                "Competition No Race",
            });

        var drivers = new CsvImport<Driver>().Read(
            new[]
            {
                "Name",
                "Maxi",
                "Seppi",
                "Franzi",
                "Anna",
                "Driver No Race",
            });

        var races = new CsvImport<Race>().Read(
            new[]
            {
                "RaceTime;CompetitionId;DriverId",
                "2023/01/08 11:42:10;1;0",
                "2024/01/08 11:42:10;0;0",
                "2024/01/08 11:43:10;0;0",
                "2024/01/08 11:44:10;0;0",
                "2024/01/08 11:45:10;0;0",
                "2024/01/08 11:46:10;0;0",
                "2024/01/08 11:47:10;0;1",
                "2024/01/08 11:48:10;0;2",
                "2024/01/08 11:49:10;0;3",
                "2024/01/08 11:50:10;0;2", // No Moves => id 9
            });

        foreach (var race in races)
        {
            race.Competition   = competitions[race.CompetitionId];
            race.CompetitionId = 0;

            race.Driver   = drivers[race.DriverId];
            race.DriverId = 0;
        }

        var moves = new CsvImport<Move>().Read(
            new[]
            {
                "No;Direction;Speed;Duration;RaceId",
                "4;3;255;204;0",
                "2;2;255;202;0",
                "3;3;255;203;0",
                "1;1;255;201;0",
                "1;3;255;201;1",
                "2;3;255;202;1",
                "3;3;255;203;1",
                "4;3;255;204;1",
                "4;3;255;204;2",
                "2;3;255;202;2",
                "3;3;255;203;2",
                "1;3;255;201;2",
                "4;3;255;204;3",
                "2;3;255;202;3",
                "3;3;255;203;3",
                "1;3;255;201;3",
                "4;3;255;204;4",
                "2;3;255;202;4",
                "3;3;255;203;4",
                "1;3;255;201;4",
                "4;3;255;204;5",
                "2;3;255;202;5",
                "3;3;255;203;5",
                "1;3;255;201;5",
                "4;3;255;204;6",
                "2;3;255;202;6",
                "3;3;255;203;6",
                "1;3;255;201;6",
                "4;3;255;204;7",
                "2;3;255;202;7",
                "3;3;255;203;7",
                "1;3;255;201;7",
                "4;3;255;204;8",
                "2;3;255;202;8",
                "3;3;255;203;8",
                "1;3;255;201;8",
            });

        foreach (var move in moves)
        {
            move.Race   = races[move.RaceId];
            move.RaceId = 0;
        }


        await dbContext.Set<Race>().AddRangeAsync(races);
        await dbContext.Set<Competition>().AddRangeAsync(competitions);
        await dbContext.Set<Driver>().AddRangeAsync(drivers);
        await dbContext.Set<Move>().AddRangeAsync(moves);
    }

    public override ApplicationDbContext CreateDbContext()
    {
        var connectString = @"Data Source = (LocalDB)\MSSQLLocalDB; Initial Catalog = Robot_Test; Integrated Security = True";

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseSqlServer(connectString, x => x.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name));

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}