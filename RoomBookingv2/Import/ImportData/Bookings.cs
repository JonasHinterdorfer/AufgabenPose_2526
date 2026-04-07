using System;

namespace Import.ImportData;

public class Bookings
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string CreditCardNumber { get; set; } = string.Empty;

    public string EmailAddress { get; set; } = string.Empty;

    public DateTime From { get; set; }

    public DateTime? To { get; set; }

    public string RoomNumber { get; set; } = string.Empty;
}