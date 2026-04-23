namespace WebAPI.Endpoints;

using Core.Contracts;
using Core.Entities;

using WebAPI.Filters;

public record RatingDto(
    int     Id,
    int     Rate,
    string? Remark,
    string  UserName,
    int     StatementId
);

public static class RatingEndpoints
{
    #region Dto-Entity Mapping

    private static Rating ToEntity(RatingDto dto)
    {
        return new Rating()
        {
            Id          = dto.Id,
            Rate        = dto.Rate,
            Remark      = dto.Remark,
            UserName    = dto.UserName,
            StatementId = dto.StatementId,
        };
    }

    private static RatingDto? ToDto(Rating? entity)
    {
        if (entity is null)
        {
            return null;
        }

        return new RatingDto(
            entity.Id,
            entity.Rate,
            entity.Remark,
            entity.UserName,
            entity.StatementId
        );
    }

    private static IList<RatingDto>? ToDto(IList<Rating>? list)
    {
        if (list is null)
        {
            return null;
        }

        return list.Select(x => ToDto(x)!).ToList();
    }

    #endregion


    public static void MapRatingEndpoints(this IEndpointRouteBuilder app, string baseRoute)
    {
        var route = app.MapGroup(baseRoute).WithTags("Rating");

        route.MapGet("", async (IUnitOfWork uow) =>
            {
                var dtos = ToDto(await uow.Ratings.GetNoTrackingAsync());
                return Results.Ok(dtos);
            })
            .WithName("GetRatings")
            .Produces<List<RatingDto>>(StatusCodes.Status200OK);

        route.MapGet("/{id:int}", async (int id, IUnitOfWork uow) =>
            {
                var dto = ToDto(await uow.Ratings.GetByIdAsync(id));

                if (dto is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Rating not found",
                        detail: $"No Rating found with ID {id}");
                }

                return Results.Ok(dto);
            })
            .WithName("GetRating")
            .Produces<RatingDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        route.MapPut("/{id:int}", async (int id, RatingDto dto, IUnitOfWork uow) =>
            {
                if (id != dto.Id)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid request",
                        detail: "The ID in the URL does not match the ID in the request body");
                }

                var entity = await uow.Ratings.GetByIdAsync(id);

                if (entity is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Rating not found",
                        detail: $"No Rating found with ID {id}");
                }

                entity.Rate     = dto.Rate;
                entity.Remark   = dto.Remark;
                entity.UserName = dto.UserName;

                await uow.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithValidation<RatingDto>()
            .WithName("UpdateRating")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);

        route.MapPost("", async (RatingDto dto, IUnitOfWork uow) =>
            {
                if (dto.Id != 0)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid request",
                        detail: "The ID in the request body must be 0");
                }

                var entity = ToEntity(dto);

                await uow.Ratings.AddAsync(entity);

                await uow.SaveChangesAsync();

                int id = entity.Id;

                return Results.Created($"{baseRoute}/{id}", ToDto(await uow.Ratings.GetByIdAsync(id)));
            })
            .WithValidation<RatingDto>()
            .WithName("AddRating")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        route.MapDelete("/{id:int}", async (int id, IUnitOfWork uow) =>
            {
                var entity = await uow.Ratings.GetByIdAsync(id);

                if (entity is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Rating not found",
                        detail: $"No Rating found with ID {id}");
                }

                uow.Ratings.Remove(entity);

                await uow.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("DeleteRating")
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status204NoContent);
    }
}
