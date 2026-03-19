namespace Core.Entities;

using System.Collections.Generic;
using Base.Core.Entities;

public class Customer : EntityObject
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public IList<Booking> Bookings { get; set; } = new List<Booking>();
}
