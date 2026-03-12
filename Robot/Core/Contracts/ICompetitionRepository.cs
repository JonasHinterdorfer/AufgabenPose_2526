namespace Core.Contracts;

using System.Collections;

using Base.Core.Contracts;

using Core.Entities;

using System.Threading.Tasks;
using System.Collections.Generic;

public interface ICompetitionRepository : IGenericRepository<Competition>
{
    // select by name
    Task<Competition?> GetByNameAsync(string competitionName);

    // returns competitions that have at least one race (with includes)
    Task<IList<Competition>> GetAllWithRace();
}