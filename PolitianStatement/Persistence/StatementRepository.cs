using Core.Contracts;
using Core.Entities;
using Core.QueryResult;

namespace Persistence;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Base.Persistence;

using Microsoft.EntityFrameworkCore;

public class StatementRepository : GenericRepository<Statement>, IStatementRepository
{
    private readonly ApplicationDbContext _dbContext;

    public StatementRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IList<StatementOverview>> GetStatementOverviewsAsync(int? categoryId)
    {
        var query = _dbContext.Statements
            .Include(s => s.Category)
            .AsNoTracking();

        if (categoryId.HasValue)
        {
            query = query.Where(s => s.CategoryId == categoryId.Value);
        }

        var statements = await query.Select(s => new
        {
            s.Id,
            s.Description,
            s.Politician,
            CategoryDescription = s.Category.Description,
            Ratings = s.Ratings.GroupBy(r => r.Rate)
                .Select(g => new { Rate = g.Key, Count = g.Count() })
                .ToList()
        }).ToListAsync();

        return statements.Select(s => new StatementOverview(
            s.Id,
            s.Description,
            s.Politician,
            s.CategoryDescription,
            s.Ratings.FirstOrDefault(r => r.Rate == 1)?.Count ?? 0,
            s.Ratings.FirstOrDefault(r => r.Rate == 2)?.Count ?? 0,
            s.Ratings.FirstOrDefault(r => r.Rate == 3)?.Count ?? 0,
            s.Ratings.FirstOrDefault(r => r.Rate == 4)?.Count ?? 0,
            s.Ratings.FirstOrDefault(r => r.Rate == 5)?.Count ?? 0
        )).ToList();
    }
}