namespace Core.Entities;

using System.Collections.Generic;

using Base.Core.Entities;

public class Room : EntityObject
{
    public required string RoomNumber { get; set; }
    public required RoomType RoomType { get; set; }
   
    public ICollection<Booking>? Bookings { get; set; }
    
    public override string ToString()
    {
        return $"Room {RoomNumber} ({RoomType})";
    }
}