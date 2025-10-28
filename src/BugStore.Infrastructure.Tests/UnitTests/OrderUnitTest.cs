using BugStore.Domain.Entities;
using BugStore.Infrastructure.Tests.Repositories;

namespace BugStore.Infrastructure.Tests.UnitTests;

public class OrderUnitTest
{
    private readonly List<Order> _db = [];
    private readonly OrderFakeRepository _repository;
    private readonly Customer _customer;

    public OrderUnitTest()
    {
        _customer = Customer.Create("John Doe", "john.doe@email.com", new DateTime(1990, 1, 1)).customer!;
        _repository = new OrderFakeRepository(_db);
    }

    [Fact]
    public async Task AddAsync_ShouldAddValidOrder()
    {
        var order = Order.Create(_customer).order!;

        var result = await _repository.AddAsync(order, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Contains(order, _db);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrder_WhenExists()
    {
        var order = Order.Create(_customer).order!;
        _db.Add(order);

        var result = await _repository.GetByIdAsync(order.Id, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Equal(order.Id, result.Data!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldFail_WhenNotFound()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetPagedByCustomerAsync_ShouldReturnCorrectPage()
    {
        for (int i = 0; i < 10; i++)
        {
            var order = Order.Create(_customer).order!;
            typeof(Order).GetProperty("CreatedAt")!.SetValue(order, DateTime.UtcNow.AddMinutes(-i));
            _db.Add(order);
        }

        var result = await _repository.GetPagedByCustomerAsync(_customer.Id, pageNumber: 2, pageSize: 3, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Equal(3, result.Items.Count());
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(4, result.TotalPages);
    }

    [Fact]
    public async Task GetPagedByCustomerAsync_ShouldReturnEmpty_WhenCustomerHasNoOrders()
    {
        var result = await _repository.GetPagedByCustomerAsync(Guid.NewGuid(), pageNumber: 1, pageSize: 5, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
    }
}
