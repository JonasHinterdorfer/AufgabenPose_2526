namespace WebAPI.Endpoints;

public record BookingDto(int Id, int RoomId, int CustomerId, System.DateTime From, System.DateTime? To);
