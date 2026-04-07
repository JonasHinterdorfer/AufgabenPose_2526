namespace Core.Contracts;

using Base.Core.Contracts;

public interface IUnitOfWork : IBaseUnitOfWork
{
    ICustomerRepository Customers { get; }
    IRoomRepository     Rooms     { get; }
    IBookingRepository  Bookings  { get; }
}