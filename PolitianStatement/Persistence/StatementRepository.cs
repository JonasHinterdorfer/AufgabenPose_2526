using Core.Contracts;
using Core.Entities;

namespace Persistence;

using Base.Persistence;

public class StatementRepository : GenericRepository<Statement>, IStatementRepository
{
    private readonly ApplicationDbContext _dbContext;

    public StatementRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }
}