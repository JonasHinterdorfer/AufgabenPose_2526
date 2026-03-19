using Core.Contracts;

namespace Persistence;

using Base.Persistence;

public class UnitOfWork : BaseUnitOfWork, IUnitOfWork
{
    public UnitOfWork(ApplicationDbContext dBContext) : base(dBContext)
    {
        Bookings  = new BookingRepository(dBContext);
        //TODO
    }

    //TODO
    public IBookingRepository  Bookings  { get; }
}