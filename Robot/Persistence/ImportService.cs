namespace Persistence;

using System;
using System.Threading.Tasks;

using Core.Contracts;
using Core.Entities;

public class ImportService : IImportService
{
    private readonly IUnitOfWork _uow;
    private readonly ICompetitionRepository _competition;
    private readonly IDriverRepository _driver;
    private readonly IRaceRepository _race;
    private readonly IMoveRepository _move;

    public ImportService(IUnitOfWork uow, ICompetitionRepository competition, IDriverRepository driver, IRaceRepository race, IMoveRepository move)
    {
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _competition = competition ?? throw new ArgumentNullException(nameof(competition));
        _driver = driver ?? throw new ArgumentNullException(nameof(driver));
        _race = race ?? throw new ArgumentNullException(nameof(race));
        _move = move ?? throw new ArgumentNullException(nameof(move));
    }

    public async Task<int> ImportRace(string driver, string competition, string moves)
    {
        if (string.IsNullOrWhiteSpace(driver)) throw new ArgumentException("driver is required", nameof(driver));
        if (string.IsNullOrWhiteSpace(competition)) throw new ArgumentException("competition is required", nameof(competition));

        // find or create competition
        var comp = await _competition.GetByNameAsync(competition);
        if (comp == null)
        {
            comp = new Competition { Name = competition };
            await _competition.AddAsync(comp);
            // don't save yet
        }

        // find or create driver
        var drv = await _driver.GetByNameAsync(driver);
        if (drv == null)
        {
            drv = new Driver { Name = driver };
            await _driver.AddAsync(drv);
        }

        // create race
        var race = new Race
        {
            Competition = comp,
            Driver = drv,
            CompetitionId = comp.Id,
            DriverId = drv.Id,
            RaceTime = DateTime.Now
        };

        await _race.AddAsync(race);

        // parse moves: expected format is comma-separated directions like "1,2,3"
        var moveTokens = string.IsNullOrWhiteSpace(moves) ? Array.Empty<string>() : moves.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        int no = 1;
        foreach (var t in moveTokens)
        {
            if (!int.TryParse(t, out var dir))
                continue;

            var m = new Move
            {
                No = no++,
                Direction = dir,
                Speed = 255,
                Duration = 200,
                Race = race
            };

            await _move.AddAsync(m);
        }

        // commit
        await _uow.SaveChangesAsync();

        return moveTokens.Length;
    }
}
