namespace Persistence.Repositories;

using Base.Persistence;

using Core.Contracts.Repositories;
using Core.Entities;

public class MoveRepository : GenericRepository<Move>, IMoveRepository
{
    public MoveRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}