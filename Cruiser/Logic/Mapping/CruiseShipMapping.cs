namespace Logic.Mapping;

using Logic.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class CruiseShipMapping
{
    private static void MapDefaults<T>(this EntityTypeBuilder<T> e)
        where T : EntityObject
    {
        e.HasIndex(x => x.Id);
        e.Property(x => x.RowVersion)
            .IsRowVersion();
    }
    
    public static void Map(this EntityTypeBuilder<CruiseShip> e)
    {
        e.MapDefaults();
        e.Property(x => x.Name)
            .AsRequiredText(255);

        e.HasIndex(x => x.Name);
        
        e.Property(x => x.Cabins)
            .HasDefaultValue(null);
        
        e.Property(x => x.Crew)
            .HasDefaultValue(null);

        e.Property(x => x.Length)
            .AsDecimal(5, 2)
            .HasDefaultValue(null);

        e.Property(x => x.Passengers)
            .HasDefaultValue(null);

        e.Property(x => x.Remark)
            .AsText(255)
            .HasDefaultValue("");

        e.Property(x => x.Tonnage)
            .HasDefaultValue(null);

        e.Property(x => x.YearOfConstruction)
            .HasDefaultValue(null);
        
        e.HasOne(x => x.ShippingCompany)
            .WithMany(x => x.CruiseShips)
            .HasForeignKey(x => x.ShippingCompanyId)
            .OnDelete(DeleteBehavior.SetNull);
    }
    
    public static void Map(this EntityTypeBuilder<ShipName> e)
    {
        e.MapDefaults();
        e.Property(x => x.Name)
            .AsRequiredText(255);

        e.HasIndex(x => x.Name);
        
        e.HasOne(x => x.CruiseShip)
            .WithMany(x => x.ShipNames)
            .HasForeignKey(x => x.CruiseShipId)
            .OnDelete(DeleteBehavior.Cascade);
    }
    
    public static void Map(this EntityTypeBuilder<ShippingCompany> e)
    {
        e.MapDefaults();
        e.Property(x => x.Name)
            .AsRequiredText(256);

        e.HasIndex(x => x.Name)
            .IsUnique();

        e.Property(x => x.City)
            .AsText(128);

        e.Property(x => x.PLZ)
            .AsText(16);

        e.Property(x => x.Street)
            .AsText(128);

        e.Property(x => x.StreetNo)
            .AsText(16);
    }
}