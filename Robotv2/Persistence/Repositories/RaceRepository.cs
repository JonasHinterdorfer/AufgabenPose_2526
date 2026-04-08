namespace Persistence.Repositories;

using Base.Persistence;

using Core.Contracts.Repositories;
using Core.Entities;

public class RaceRepository : GenericRepository<Race>, IRaceRepository
{
    public RaceRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}