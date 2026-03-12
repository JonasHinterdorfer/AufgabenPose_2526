namespace Persistence;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Base.Persistence;

using Core.Contracts;
using Core.Entities;

public class CompetitionRepository : GenericRepository<Competition>, ICompetitionRepository
{
    public CompetitionRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Competition?> GetByNameAsync(string competitionName)
    {
        var list = await GetNoTrackingAsync(c => c.Name == competitionName);
        return list.Count == 0 ? null : list[0];
    }

    public Task<IList<Competition>> GetAllWithRace()
    {
        // return competitions that have at least one race and include nested Driver and Moves
        return GetAsync(c => c.Races.Any(), null, "Races.Driver", "Races.Moves");
    }
}