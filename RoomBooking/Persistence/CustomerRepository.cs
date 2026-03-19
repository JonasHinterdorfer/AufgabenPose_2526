using Core.Contracts;
using Core.Entities;
using Core.QueryResult;

using Microsoft.EntityFrameworkCore;

namespace Persistence;

using Base.Persistence;

public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CustomerRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IList<CustomerOverview>> GetAllAsync(string? filterName, bool? onlyWithBookings)
    {
        var query = _dbContext.Customers.AsQueryable();

        if (!string.IsNullOrEmpty(filterName))
        {
            var f = filterName.ToLower();
            query = query.Where(c => (c.FirstName + " " + c.LastName).ToLower().Contains(f));
        }

        if (onlyWithBookings == true)
        {
            var today = DateTime.Today;
            query = query.Where(c => c.Bookings.Any(b => b.From <= today && (b.To == null || b.To >= today)));
        }

        var list = await query
            .Select(c => new CustomerOverview(c.Id, c.FirstName, c.LastName, c.Bookings.Count))
            .ToListAsync();

        return list;
    }

    public async Task<List<Booking>> GetBookingsForCustomer(int id)
    {
        return await _dbContext.Bookings
            .Where(b => b.CustomerId == id)
            .OrderByDescending(b => b.From)
            .ToListAsync();
    }
}
