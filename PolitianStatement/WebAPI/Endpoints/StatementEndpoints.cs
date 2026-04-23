namespace WebAPI.Endpoints;

using Core.Contracts;
using Core.Entities;
using Core.QueryResult;

using WebAPI.Filters;

public record StatementDto(
    int    Id,
    string Description,
    string StatementType,
    string Category,
    string Politician
);

public static class StatementEndpoints
{
    #region Dto-Entity Mapping

    private static Statement ToEntity(StatementDto dto)
    {
        return new Statement()
        {
            Id            = dto.Id,
            Description   = dto.Description,
            Politician    = dto.Politician,
            StatementType = Enum.Parse<Core.Entities.StatementType>(dto.StatementType, ignoreCase: true),
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
            entity.Description,
            entity.StatementType.ToString(),
            entity.Category?.Description ?? string.Empty,
            entity.Politician
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

        route.MapGet("/overview", async (int? category, IUnitOfWork uow) =>
            {
                var overviews = await uow.Statements.GetStatementOverviewsAsync(category);
                return Results.Ok(overviews);
            })
            .WithName("GetStatementOverview")
            .Produces<List<StatementOverview>>(StatusCodes.Status200OK);

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

                entity.Description   = dto.Description;
                entity.Politician    = dto.Politician;
                entity.StatementType = Enum.Parse<Core.Entities.StatementType>(dto.StatementType, ignoreCase: true);
                entity.Modified      = DateTime.UtcNow;

                await uow.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithValidation<StatementDto>()
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

                var categoryEntities = await uow.Categories.GetAsync(c => c.Description == dto.Category);
                var category = categoryEntities.FirstOrDefault();

                if (category is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Category not found",
                        detail: $"No Category found with description '{dto.Category}'");
                }

                var entity = ToEntity(dto);
                entity.CategoryId = category.Id;
                entity.Created    = DateTime.UtcNow;

                await uow.Statements.AddAsync(entity);

                await uow.SaveChangesAsync();

                int id = entity.Id;

                return Results.Created($"{baseRoute}/{id}", ToDto(await uow.Statements.GetByIdAsync(id, nameof(Statement.Category))));
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