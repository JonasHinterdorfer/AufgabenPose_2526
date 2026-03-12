namespace Persistence;

using Core.Contracts;
using Core.Entities;

using Base.Persistence;

public class MoveRepository : GenericRepository<Move>, IMoveRepository
{
    public MoveRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}
