using Core.Contracts;
using Core.Entities;

namespace WebAPI.Endpoints;

using WebAPI.Filters;


public record CustomerDto(int Id, string FirstName, string LastName, string IBAN);

public static class CustomerEndpoints
{
    #region Dto-Entity Mapping

    private static Customer ToEntity(CustomerDto dto)
    {
        return new Customer
        {
            Id         = dto.Id,
            FirstName  = dto.FirstName,
            LastName   = dto.LastName,
            IBAN       = dto.IBAN,
        };
    }

    private static CustomerDto? ToDto(Customer? entity)
    {
        if (entity is null)
        {
            return null;
        }

        return new CustomerDto(
            entity.Id,
            entity.FirstName,
            entity.LastName,
            entity.IBAN
        );
    }

    private static IList<CustomerDto>? ToDto(IList<Customer>? list)
    {
        if (list is null)
        {
            return null;
        }

        return list.Select(x => ToDto(x)!).ToList();
    }

    #endregion


    public static void MapCustomerEndpoints(this IEndpointRouteBuilder app, string baseRoute)
    {
        var route = app.MapGroup(baseRoute).WithTags("Customer");

        route.MapGet("", async (IUnitOfWork uow) =>
            {
                var dtos = ToDto(await uow.Customers.GetNoTrackingAsync());
                return Results.Ok(dtos);
            })
            .WithName("GetCustomers")
            .Produces<List<CustomerDto>>(StatusCodes.Status200OK);

        route.MapGet("/{id:int}", async (int id, IUnitOfWork uow) =>
            {
                var dto = ToDto(await uow.Customers.GetByIdAsync(id));

                if (dto is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status404NotFound,
                        title: "Customer not found",
                        detail: $"No Customer found with ID {id}");
                }

                return Results.Ok(dto);
            })
            .WithName("GetCustomer")
            .Produces<CustomerDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);

        route.MapPut("/{id:int}", async (int id, CustomerDto dto, IUnitOfWork uow) =>
            {
                if (id != dto.Id)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid request",
                        detail: "The ID in the URL does not match the ID in the request body");
                }

                var entity = await uow.Customers.GetByIdAsync(id);

                if (entity is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Customer not found",
                        detail: $"No Customer found with ID {id}");
                }

                entity.FirstName = dto.FirstName;
                entity.LastName = dto.LastName;
                entity.IBAN = dto.IBAN;

                await uow.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithValidation<CustomerDto>()
            .WithName("UpdateCustomer")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);


        route.MapPost("", async (CustomerDto dto, IUnitOfWork uow) =>
            {
                if (dto.Id != 0)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Invalid request",
                        detail: "The ID in the request body must be 0");
                }

                var entity = ToEntity(dto);

                await uow.Customers.AddAsync(entity);

                await uow.SaveChangesAsync();

                int id = entity.Id;

                return Results.Created($"{baseRoute}/{id}", await uow.Customers.GetByIdAsync(id));
            })
            .WithValidation<CustomerDto>()
            .WithName("AddCustomer")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);

        route.MapDelete("/{id:int}", async (int id, IUnitOfWork uow) =>
            {
                var entity = await uow.Customers.GetByIdAsync(id);

                if (entity is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Customer not found",
                        detail: $"No Customer found with ID {id}");
                }

                uow.Customers.Remove(entity);

                await uow.SaveChangesAsync();

                return Results.NoContent();
            })
            .WithName("DeleteCustomer")
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status204NoContent);
    }
}