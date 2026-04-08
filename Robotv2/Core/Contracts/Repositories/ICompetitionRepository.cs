namespace Core.Contracts.Repositories;

using System.Threading.Tasks;

using Base.Core.Contracts;

using Core.Entities;

public interface ICompetitionRepository : IGenericRepository<Competition>
{
    public Task<Competition?> GetByNameAsync(string competitionName);
}