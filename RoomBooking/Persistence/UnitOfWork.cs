using Core.Contracts;

namespace Persistence;

using Base.Persistence;
using Base.Core.Contracts;
using Core.Entities;

public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
{
    public UnitOfWork(ApplicationDbContext dBContext) : base(dBContext)
    {
        Bookings  = new BookingRepository(dBContext);
        Rooms     = new RoomRepository(dBContext);
        Customers = new CustomerRepository(dBContext);
    }

    public IBookingRepository  Bookings  { get; }
    public IRoomRepository Rooms { get; }
    public ICustomerRepository Customers { get; }
}