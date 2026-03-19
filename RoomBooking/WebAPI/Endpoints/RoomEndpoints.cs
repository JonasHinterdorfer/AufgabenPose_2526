using Core.Contracts;
using Core.Entities;

namespace WebAPI.Endpoints;

using WebAPI.Filters;

public record RoomDto(
    int    Id,
    string RoomNumber,
    string RoomType
);

public static class RoomEndpoints
{
    #region Dto-Entity Mapping

    private static Room ToEntity(RoomDto dto)
    {
        return new Room()
        {
            Id         = dto.Id,
            RoomNumber = dto.RoomNumber,
            RoomType   = Enum.Parse<RoomType>(dto.RoomType),
        };
    }

    private static RoomDto? ToDto(Room? entity)
    {
        if (entity is null)
        {
            return null;
        }

        return new RoomDto(
            entity.Id,
            entity.RoomNumber,
            entity.RoomType.ToString()
        );
    }

    private static IList<RoomDto>? ToDto(IList<Room>? list)
    {
        if (list is null)
        {
            return null;
        }

        return list.Select(x => ToDto(x)!).ToList();
    }

    #endregion


    public static void MapRoomEndpoints(this IEndpointRouteBuilder app, string baseRoute)
    {
        var route = app.MapGroup(baseRoute).WithTags("Room");

        route.MapGet("", async (IUnitOfWork uow) =>
            {
                var dtos = ToDto(await uow.Rooms.GetNoTrackingAsync());
                return Results.Ok(dtos);
            })
            .WithName("GetRooms")
            .Produces<List<RoomDto>>(StatusCodes.Status200OK);

        route.MapGet("/{id:int}", async (int id, IUnitOfWork uow) =>
            {
                var dto = ToDto(await uow.Rooms.GetByIdAsync(id));

                if (dto is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Room not found",
                        detail: $"No Room found with ID {id}");
                }

                return Results.Ok(dto);
            })
            .WithName("GetRoom")
            .Produces<RoomDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        route.MapPut("/{id:int}", async (int id, RoomDto dto, IUnitOfWork uow) =>
            {
                if (id != dto.Id)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid request",
                        detail: "The ID in the URL does not match the ID in the request body");
                }

                var entity = await uow.Rooms.GetByIdAsync(id);

                if (entity is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Room not found",
                        detail: $"No Room found with ID {id}");
                }

                entity.RoomNumber = dto.RoomNumber;
                entity.RoomType   = Enum.Parse<RoomType>(dto.RoomType);

                await uow.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithValidation<RoomDto>()
            .WithName("UpdateRoom")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);


        route.MapPost("", async (RoomDto dto, IUnitOfWork uow) =>
            {
                if (dto.Id != 0)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid request",
                        detail: "The ID in the request body must be 0");
                }

                var entity = ToEntity(dto);

                await uow.Rooms.AddAsync(entity);

                await uow.SaveChangesAsync();

                int id = entity.Id;

                return Results.Created($"{baseRoute}/{id}", await uow.Rooms.GetByIdAsync(id));
            })
            .WithValidation<RoomDto>()
            .WithName("AddRoom")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        route.MapDelete("/{id:int}", async (int id, IUnitOfWork uow) =>
            {
                var entity = await uow.Rooms.GetByIdAsync(id);

                if (entity is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Room not found",
                        detail: $"No Room found with ID {id}");
                }

                uow.Rooms.Remove(entity);

                await uow.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("DeleteRoom")
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status204NoContent);

        route.MapGet("/overview", async (RoomType? roomType, string? filterNumber, IUnitOfWork uow) =>
            {
                var overviews = await uow.Rooms.GetRoomWithBookingsAsync(roomType, filterNumber);
                return Results.Ok(overviews);
            })
            .WithName("GetRoomOverview")
            .Produces<List<Core.QueryResult.RoomOverview>>(StatusCodes.Status200OK);
    }
}