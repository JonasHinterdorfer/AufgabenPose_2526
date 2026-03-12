namespace Core.Contracts;

using Base.Core.Contracts;

using Core.Entities;

using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRaceRepository : IGenericRepository<Race>
{
    Task<IList<Race>> GetByDriverAsync(int driverId);
    Task<IList<Race>> GetByCompetitionAsync(int competitionId);
    Task<int> GetRaceCount(string driverName, string competitionName);
}
