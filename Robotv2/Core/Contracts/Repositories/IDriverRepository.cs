namespace Core.Contracts.Repositories;

using System.Threading.Tasks;

using Base.Core.Contracts;

using Core.Entities;

public interface IDriverRepository : IGenericRepository<Driver>
{

    public Task<Driver?> GetByNameAsync(string competitionName);
}