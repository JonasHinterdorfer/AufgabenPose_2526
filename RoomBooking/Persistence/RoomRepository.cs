using Core.Contracts;
using Core.Entities;
using Core.QueryResult;

using Microsoft.EntityFrameworkCore;

namespace Persistence;

using Base.Persistence;

public class RoomRepository : GenericRepository<Room>, IRoomRepository
{
    private readonly ApplicationDbContext _dbContext;

    public RoomRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IList<RoomOverview>> GetRoomWithBookingsAsync(RoomType? roomType, string? filterNumber)
    {
        var query = _dbContext.Rooms.Include(r => r.Bookings).AsQueryable();

        if (roomType.HasValue)
        {
            query = query.Where(r => r.RoomType == roomType.Value);
        }

        if (!string.IsNullOrEmpty(filterNumber))
        {
            query = query.Where(r => r.RoomNumber.Contains(filterNumber));
        }

        var today = DateTime.Today;

        var list = await query
            .OrderBy(r => r.RoomNumber)
            .Select(r => new RoomOverview(
                r.RoomNumber,
                r.RoomType.ToString(),
                !r.Bookings.Any(b => b.From <= today && (b.To == null || b.To >= today))
            ))
            .ToListAsync();

        return list;
    }
}
