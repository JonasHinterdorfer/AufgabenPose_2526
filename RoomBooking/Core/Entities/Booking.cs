using Core.Validations;

using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

using System;

using Base.Core.Entities;

public class Booking : EntityObject
{
    // Link to Room
    public int RoomId { get; set; }
    public Core.Entities.Room? Room { get; set; }

    // Link to Customer
    public int CustomerId { get; set; }
    public Core.Entities.Customer? Customer { get; set; }

    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}