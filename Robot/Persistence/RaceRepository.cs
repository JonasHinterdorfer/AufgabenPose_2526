namespace Persistence;

using System.Collections.Generic;
using System.Threading.Tasks;

using Core.Contracts;
using Core.Entities;

using Base.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class RaceRepository : GenericRepository<Race>, IRaceRepository
{
    public RaceRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<IList<Race>> GetByDriverAsync(int driverId)
    {
        return GetAsync(r => r.DriverId == driverId, null, "Driver", "Moves", "Competition");
    }

    public Task<IList<Race>> GetByCompetitionAsync(int competitionId)
    {
        return GetAsync(r => r.CompetitionId == competitionId, null, "Driver", "Moves", "Competition");
    }

    public async Task<int> GetRaceCount(string driverName, string competitionName)
    {
        // Count races where race.Driver.Name == driverName (if provided) and race.Competition.Name == competitionName (if provided)
        var query = Context.Set<Race>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(driverName))
        {
            query = query.Where(r => r.Driver != null && r.Driver.Name == driverName);
        }

        if (!string.IsNullOrWhiteSpace(competitionName))
        {
            query = query.Where(r => r.Competition != null && r.Competition.Name == competitionName);
        }

        return await query.CountAsync();
    }
}
