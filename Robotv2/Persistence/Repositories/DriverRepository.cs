namespace Persistence.Repositories;

using System.Threading.Tasks;

using Base.Persistence;

using Core.Contracts.Repositories;
using Core.Entities;

using Microsoft.EntityFrameworkCore;

public class DriverRepository : GenericRepository<Driver>, IDriverRepository 
{
    public DriverRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
    
    public Task<Driver?> GetByNameAsync(string competitionName)
    {
        return DbSet.FirstOrDefaultAsync(x => x.Name == competitionName);
    }
}