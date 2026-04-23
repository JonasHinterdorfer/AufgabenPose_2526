using Core.Contracts;
using Core.Entities;

namespace Persistence;

using Base.Persistence;

public class RatingRepository : GenericRepository<Rating>, IRatingRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RatingRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}
