using BugStore.Application.Abstractions.Repositories;
using BugStore.Application.Common;
using BugStore.Domain.Entities;

namespace BugStore.Application.Tests.Repositories;

public class FakeOrderRepository(List<Order> db) : IOrderRepository
{
    public async Task<Result<Order>> AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!order.IsValid)
                return Result<Order>.Fail("INVALID_ENTITY: Order is not valid");

            db.Add(order);

      return Result<Order>.Ok(order);
        }
        catch (Exception ex)
        {
            return Result<Order>.Fail($"GENERIC: Unexpected error while adding order - {ex.Message}");
        }
    }

    public async Task<Result<Order>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
 var order = db.FirstOrDefault(o => o.Id == id);

    return order is not null
             ? Result<Order>.Ok(order)
       : Result<Order>.Fail("NOT_FOUND: Order not found");
        }
        catch (Exception ex)
     {
            return Result<Order>.Fail($"GENERIC: Unexpected error while retrieving order: {ex.Message}");
        }
    }

    public async Task<PagedResult<Order>> GetPagedByCustomerAsync(Guid customerId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        try
        {
 var totalCount = db
      .Where(o => o.CustomerId == customerId)
                .Count();

    var items = db
    .Where(o => o.CustomerId == customerId)
     .OrderByDescending(o => o.CreatedAt)
       .Skip((pageNumber - 1) * pageSize)
       .Take(pageSize)
          .ToList();

    return PagedResult<Order>.Ok(items, totalCount, pageNumber, pageSize);
        }
     catch (Exception ex)
{
          return PagedResult<Order>.Fail($"GENERIC: Unexpected error while paging orders: {ex.Message}");
 }
    }
}
