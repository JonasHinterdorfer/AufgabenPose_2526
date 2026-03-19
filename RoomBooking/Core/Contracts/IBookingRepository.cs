using Core.Entities;

namespace Core.Contracts;

using System.Collections.Generic;
using System.Threading.Tasks;

using Base.Core.Contracts;

public interface IBookingRepository : IGenericRepository<Booking>
{
    // Booking-specific queries can be added here if needed
}