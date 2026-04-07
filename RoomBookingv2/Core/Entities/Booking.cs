using Core.Validations;

using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

using System;

using Base.Core.Entities;

public class Booking : EntityObject
{
    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }
    
    public int RoomId { get; set; }
    public Room? Room { get; set; }
    
    public DateTime From { get; set; }
    public DateTime? To { get; set; }
}