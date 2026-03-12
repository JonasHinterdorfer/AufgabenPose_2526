﻿namespace WebApi.Endpoints;

using Logic;
using Logic.Entities;

using Microsoft.EntityFrameworkCore;

public static class CruiseShipEndpoints
{
    public static void MapCruiseShipEndpoints(this IEndpointRouteBuilder app, string baseRoute)
    {
        var ships = app.MapGroup($"{baseRoute}").WithTags("CruiseShips");

        ships.MapGet("", async (ApplicationDbContext dbContext) =>
            {
                var result = await dbContext.Ships
                    .Include(s => s.ShippingCompany)
                    .Include(s => s.ShipNames)
                    .Select(s => new
                    {
                        s.Id,
                        s.Name,
                        s.Cabins,
                        s.Crew,
                        s.Length,
                        s.Passengers,
                        s.Remark,
                        s.Tonnage,
                        s.YearOfConstruction,
                        ShippingCompany = s.ShippingCompany != null ? s.ShippingCompany.Name : null,
                        ShipNames = s.ShipNames != null ? s.ShipNames.Select(sn => sn.Name).ToList() : []
                    })
                    .ToListAsync();

                return Results.Ok(result);
            })
            .WithName("GetCruiseShips")
            .Produces(StatusCodes.Status200OK);
    }
}