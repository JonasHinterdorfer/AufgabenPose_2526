using Core.Contracts;
using Core.Entities;
using Core.QueryResult;

namespace WebAPI.Endpoints;

using WebAPI.Filters;

public static class CustomerEndpoints
{
    private static Customer ToEntity(CustomerDto dto)
    {
        return new Customer
        {
            Id = dto.Id,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email
        };
    }

    private static CustomerDto? ToDto(Customer? entity)
    {
        if (entity is null) return null;
        return new CustomerDto(entity.Id, entity.FirstName, entity.LastName, entity.Email);
    }

    private static IList<CustomerDto>? ToDto(IList<Customer>? list)
    {
        if (list is null) return null;
        return list.Select(x => ToDto(x)!).ToList();
    }

    private static BookingDto ToBookingDto(Booking b)
    {
        return new BookingDto(b.Id, b.RoomId, b.CustomerId, b.From, b.To);
    }

    public static void MapCustomerEndpoints(this IEndpointRouteBuilder app, string baseRoute)
    {
        var route = app.MapGroup(baseRoute).WithTags("Customer");

        route.MapGet("", async (IUnitOfWork uow) =>
        {
            var entities = await uow.Customers.GetNoTrackingAsync();
            return Results.Ok(ToDto(entities));
        })
        .WithName("GetCustomers")
        .Produces<List<CustomerDto>>(StatusCodes.Status200OK);

        route.MapGet("/{id:int}", async (int id, IUnitOfWork uow) =>
        {
            var dto = ToDto(await uow.Customers.GetByIdAsync(id));
            if (dto is null) return Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Customer not found");
            return Results.Ok(dto);
        })
        .WithName("GetCustomer").Produces<CustomerDto>(StatusCodes.Status200OK);

        route.MapPost("", async (CustomerDto dto, IUnitOfWork uow) =>
        {
            if (dto.Id != 0) return Results.Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid request", detail: "The ID in the request body must be 0");
            var entity = ToEntity(dto);
            await uow.Customers.AddAsync(entity);
            await uow.SaveChangesAsync();
            return Results.Created($"{baseRoute}/{entity.Id}", entity);
        })
        .WithValidation<CustomerDto>()
        .WithName("AddCustomer");

        route.MapPut("/{id:int}", async (int id, CustomerDto dto, IUnitOfWork uow) =>
        {
            if (id != dto.Id) return Results.Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid request", detail: "The ID in the URL does not match the ID in the request body");
            var entity = await uow.Customers.GetByIdAsync(id);
            if (entity is null) return Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Customer not found");
            entity.FirstName = dto.FirstName;
            entity.LastName = dto.LastName;
            entity.Email = dto.Email;
            await uow.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithValidation<CustomerDto>()
        .WithName("UpdateCustomer");

        route.MapDelete("/{id:int}", async (int id, IUnitOfWork uow) =>
        {
            var entity = await uow.Customers.GetByIdAsync(id);
            if (entity is null) return Results.Problem(statusCode: StatusCodes.Status404NotFound, title: "Customer not found");
            uow.Customers.Remove(entity);
            await uow.SaveChangesAsync();
            return Results.NoContent();
        })
        .WithName("DeleteCustomer");

        route.MapGet("/overview", async (string? filterName, bool? onlyWithBookings, IUnitOfWork uow) =>
        {
            var overviews = await uow.Customers.GetAllAsync(filterName, onlyWithBookings);
            return Results.Ok(overviews);
        })
        .WithName("GetCustomerOverview")
        .Produces<List<CustomerOverview>>(StatusCodes.Status200OK);

        route.MapGet("/{id:int}/bookings", async (int id, IUnitOfWork uow) =>
        {
            var bookings = await uow.Customers.GetBookingsForCustomer(id);
            return Results.Ok(bookings.Select(b => ToBookingDto(b)).ToList());
        })
        .WithName("GetBookingsForCustomer")
        .Produces<List<BookingDto>>(StatusCodes.Status200OK);
    }
}
