namespace Logic.Entities;

public class CruiseShip : EntityObject
{
    public required string Name   { get; set; }
    
    public int?     Cabins             { get; set; }
    public int?     Crew               { get; set; }
    public decimal? Length             { get; set; }
    public int?     Passengers         { get; set; }
    public string   Remark             { get; set; } = String.Empty;
    public int?     Tonnage            { get; set; }
    public int?     YearOfConstruction { get; set; }
    
    public ICollection<ShipName>? ShipNames { get; set; }
    
    public int? ShippingCompanyId { get; set; }
    public ShippingCompany? ShippingCompany { get; set; }
}