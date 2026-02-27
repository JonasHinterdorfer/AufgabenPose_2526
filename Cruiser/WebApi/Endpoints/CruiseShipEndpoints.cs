namespace WebApi.Endpoints;

using Logic;
using Logic.Entities;

using Microsoft.EntityFrameworkCore;

public static class CruiseShipEndpoints
{
    public static void MapCruiseShipEndpoints(this IEndpointRouteBuilder app, string baseRoute)
    {
        var ships = app.MapGroup($"{baseRoute}").WithTags("CruiseShips");

        // GET: api/CruiseShips
        ships.MapGet("", async () =>
            {
                //TODO get Data and map to DTOs
                throw new NotImplementedException();
            })
            .WithName("GetCruiseShips")
            .Produces<List<CruiseShip>>(StatusCodes.Status200OK);
    }
}