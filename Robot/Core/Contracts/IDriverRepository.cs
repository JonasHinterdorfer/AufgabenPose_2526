namespace Core.Contracts;

using System.Threading.Tasks;

using Base.Core.Contracts;

using Core.Entities;

public interface IDriverRepository : IGenericRepository<Driver>
{
    Task<Driver?> GetByNameAsync(string driverName);
}
