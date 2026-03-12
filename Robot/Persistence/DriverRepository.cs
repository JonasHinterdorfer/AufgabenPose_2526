namespace Persistence;

using System.Threading.Tasks;

using Core.Contracts;
using Core.Entities;

using Base.Persistence;

public class DriverRepository : GenericRepository<Driver>, IDriverRepository
{
    public DriverRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Driver?> GetByNameAsync(string driverName)
    {
        var list = await GetNoTrackingAsync(d => d.Name == driverName);
        return list.Count == 0 ? null : list[0];
    }
}
