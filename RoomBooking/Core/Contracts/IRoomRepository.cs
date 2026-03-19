using Core.QueryResult;
using Core.Entities;

namespace Core.Contracts;

using System.Collections.Generic;
using System.Threading.Tasks;

using Base.Core.Contracts;

public interface IRoomRepository : IGenericRepository<Room>
{
    Task<IList<RoomOverview>> GetRoomWithBookingsAsync(RoomType? roomType, string? filterNumber);
}
