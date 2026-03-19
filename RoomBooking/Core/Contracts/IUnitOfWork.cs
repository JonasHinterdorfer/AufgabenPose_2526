namespace Core.Contracts;

using Base.Core.Contracts;

using Core.Entities;

public interface IUnitOfWork : IBaseUnitOfWork
{
    IRoomRepository Rooms { get; }
    ICustomerRepository Customers { get; }
    IBookingRepository Bookings { get; }
}