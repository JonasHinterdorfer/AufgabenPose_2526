namespace Logic.Entities;

public class CruiseShip : EntityObject
{
    public required string Name   { get; set; }
    
    public int?  Cabins     { get; set; }
    public int?  Crew       { get; set; }
    public uint? Length     { get; set; }
    public int?  Passengers { get; set; }
    public string? Remark { get; set; }
    public int? Tonnage  { get; set; }
    public int? YearOfConstruction { get; set; }
}