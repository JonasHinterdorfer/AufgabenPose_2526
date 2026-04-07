using Core.Contracts;

namespace Persistence;

using Base.Persistence;

public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
{
    public UnitOfWork(ApplicationDbContext dBContext) : base(dBContext)
    {
        Bookings  = new BookingRepository(dBContext);
        Customers = new CustomerRepository(dBContext);
        Rooms      = new RoomRepository(dBContext);
    }

    public IRoomRepository Rooms { get; set; }
    public IBookingRepository Bookings { get; }
    public ICustomerRepository Customers { get; }
}