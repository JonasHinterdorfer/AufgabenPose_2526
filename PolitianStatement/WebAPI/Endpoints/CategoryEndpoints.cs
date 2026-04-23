namespace WebAPI.Endpoints;

using Core.Contracts;
using Core.Entities;

using WebAPI.Filters;

public record CategoryDto(
    int    Id,
    string Description
);

public static class CategoryEndpoints
{
    #region Dto-Entity Mapping

    private static Category ToEntity(CategoryDto dto)
    {
        return new Category()
        {
            Id          = dto.Id,
            Description = dto.Description
        };
    }

    private static CategoryDto? ToDto(Category? entity)
    {
        if (entity is null)
        {
            return null;
        }

        return new CategoryDto(
            entity.Id,
            entity.Description
        );
    }

    private static IList<CategoryDto>? ToDto(IList<Category>? list)
    {
        if (list is null)
        {
            return null;
        }

        return list.Select(x => ToDto(x)!).ToList();
    }

    #endregion


    public static void MapCategoryEndpoints(this IEndpointRouteBuilder app, string baseRoute)
    {
        var route = app.MapGroup(baseRoute).WithTags("Category");

        route.MapGet("", async (IUnitOfWork uow) =>
            {
                var dtos = ToDto(await uow.Categories.GetNoTrackingAsync());
                return Results.Ok(dtos);
            })
            .WithName("GetCategories")
            .Produces<List<CategoryDto>>(StatusCodes.Status200OK);

        route.MapGet("/{id:int}", async (int id, IUnitOfWork uow) =>
            {
                var dto = ToDto(await uow.Categories.GetByIdAsync(id));

                if (dto is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Category not found",
                        detail: $"No Category found with ID {id}");
                }

                return Results.Ok(dto);
            })
            .WithName("GetCategory")
            .Produces<CategoryDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        route.MapPut("/{id:int}", async (int id, CategoryDto dto, IUnitOfWork uow) =>
            {
                if (id != dto.Id)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid request",
                        detail: "The ID in the URL does not match the ID in the request body");
                }

                var entity = await uow.Categories.GetByIdAsync(id);

                if (entity is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Category not found",
                        detail: $"No Category found with ID {id}");
                }

                entity.Description = dto.Description;

                await uow.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithValidation<CategoryDto>()
            .WithName("UpdateCategory")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);


        route.MapPost("", async (CategoryDto dto, IUnitOfWork uow) =>
            {
                if (dto.Id != 0)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid request",
                        detail: "The ID in the request body must be 0");
                }

                var entity = ToEntity(dto);

                await uow.Categories.AddAsync(entity);

                await uow.SaveChangesAsync();

                int id = entity.Id;

                return Results.Created($"{baseRoute}/{id}", ToDto(await uow.Categories.GetByIdAsync(id)));
            })
            .WithValidation<CategoryDto>()
            .WithName("AddCategory")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        route.MapDelete("/{id:int}", async (int id, IUnitOfWork uow) =>
            {
                var entity = await uow.Categories.GetByIdAsync(id);

                if (entity is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Category not found",
                        detail: $"No Category found with ID {id}");
                }

                uow.Categories.Remove(entity);

                await uow.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("DeleteCategory")
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status204NoContent);
    }
}