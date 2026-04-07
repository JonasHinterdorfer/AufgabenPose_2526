namespace Core.Entities;

using System.Collections.Generic;

using Base.Core.Entities;

public class Customer: EntityObject
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string IBAN { get; set; }
    
    public ICollection<Booking>? Bookings { get; set; }

    public override string ToString()
    {
        return $"{FirstName} {LastName}, IBAN: {IBAN}";
    }
}