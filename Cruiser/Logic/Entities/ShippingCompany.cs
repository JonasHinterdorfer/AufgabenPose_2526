namespace Logic.Entities;

public class ShippingCompany : EntityObject
{

    public required string  Name     { get; set; }
    public          string? City     { get; set; }
    public          string? PLZ      { get; set; }
    public          string? Street   { get; set; }
    public          int?     StreetNo { get; set; }

    public ICollection<CruiseShip>? CruiseShips { get; set; } = null;
}