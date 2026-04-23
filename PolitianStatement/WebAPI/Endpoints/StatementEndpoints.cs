namespace WebAPI.Endpoints;

using Core.Contracts;
using Core.Entities;
using Core.QueryResult;

using WebAPI.Filters;

public record StatementDto(
);

public static class StatementEndpoints
{
    #region Dto-Entity Mapping

    private static Statement ToEntity(StatementDto dto)
    {
        return new Statement()
        {
            Id          = dto.Id,
        };
    }

    private static StatementDto? ToDto(Statement? entity)
    {
        if (entity is null)
        {
            return null;
        }

        return new StatementDto(
            entity.Id,
        );
    }

    private static IList<StatementDto>? ToDto(IList<Statement>? list)
    {
        if (list is null)
        {
            return null;
        }

        return list.Select(x => ToDto(x)!).ToList();
    }

    #endregion


    public static void MapStatementEndpoints(this IEndpointRouteBuilder app, string baseRoute)
    {
        var route = app.MapGroup(baseRoute).WithTags("Statement");

        route.MapGet("", async (IUnitOfWork uow) =>
            {
                var dtos = ToDto(await uow.Statements.GetNoTrackingAsync(null, null, nameof(Statement.Category)));
                return Results.Ok(dtos);
            })
            .WithName("GetStatements")
            .Produces<List<StatementDto>>(StatusCodes.Status200OK);


        route.MapGet("/{id:int}", async (int id, IUnitOfWork uow) =>
            {
                var dto = ToDto(await uow.Statements.GetByIdAsync(id, nameof(Statement.Category)));

                if (dto is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Statement not found",
                        detail: $"No Statement found with ID {id}");
                }

                return Results.Ok(dto);
            })
            .WithName("GetStatement")
            .Produces<StatementDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        route.MapPut("/{id:int}", async (int id, StatementDto dto, IUnitOfWork uow) =>
            {
                if (id != dto.Id)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid request",
                        detail: "The ID in the URL does not match the ID in the request body");
                }

                var entity = await uow.Statements.GetByIdAsync(id);

                if (entity is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Statement not found",
                        detail: $"No Statement found with ID {id}");
                }

                throw new NotImplementedException();

                await uow.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("UpdateStatement")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);


        route.MapPost("", async (StatementDto dto, IUnitOfWork uow) =>
            {
                if (dto.Id != 0)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid request",
                        detail: "The ID in the request body must be 0");
                }

                var entity = ToEntity(dto);

                throw new NotImplementedException();

                await uow.Statements.AddAsync(entity);

                await uow.SaveChangesAsync();

                int id = entity.Id;

                return Results.Created($"{baseRoute}/{id}", ToDto(await uow.Statements.GetByIdAsync(id)));
            })
            .WithValidation<StatementDto>()
            .WithName("AddStatement")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        route.MapDelete("/{id:int}", async (int id, IUnitOfWork uow) =>
            {
                var entity = await uow.Statements.GetByIdAsync(id);

                if (entity is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Statement not found",
                        detail: $"No Statement found with ID {id}");
                }

                uow.Statements.Remove(entity);

                await uow.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("DeleteStatement")
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status204NoContent);
    }
}