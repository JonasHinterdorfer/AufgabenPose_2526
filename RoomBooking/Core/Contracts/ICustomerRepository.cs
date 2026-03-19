using Core.QueryResult;
using Core.Entities;

namespace Core.Contracts;

using System.Collections.Generic;
using System.Threading.Tasks;

using Base.Core.Contracts;

public interface ICustomerRepository : IGenericRepository<Customer>
{
    Task<IList<CustomerOverview>> GetAllAsync(string? filterName, bool? onlyWithBookings);
    Task<List<Booking>> GetBookingsForCustomer(int id);
}
