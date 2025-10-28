using BugStore.Domain.Entities;
using BugStore.Infrastructure.Tests.Repositories;

namespace BugStore.Infrastructure.Tests.UnitTests;

public class CustomerUnitTest
{
    private readonly List<Customer> _db = [];
    private readonly FakeCustomerRepository _repository;

    public CustomerUnitTest()
    {
        _repository = new FakeCustomerRepository(_db);
    }

    [Fact]
    public async Task AddAsync_ShouldAddValidCustomer()
    {
        var (customer, _) = Customer.Create("John Doe", "john.doe@email.com", new DateTime(1990, 1, 1));

        var result = await _repository.AddAsync(customer!, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Contains(customer, _db);
    }

    [Fact]
    public async Task AddAsync_ShouldFail_WhenCustomerIsInvalid()
    {
        var (customer, _) = Customer.Create("", "", new DateTime(1990, 1, 1));

        var result = await _repository.AddAsync(customer, TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.DoesNotContain(customer, _db);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnCustomer_WhenExists()
    {
        var (customer, _) = Customer.Create("Ana", "ana@email.com", new DateTime(1990, 1, 1));
        _db.Add(customer!);

        var result = await _repository.GetByIdAsync(customer!.Id, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Equal(customer.Id, result.Data!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldFail_WhenNotFound()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnCustomer_WhenExists()
    {
        var (customer, _) = Customer.Create("Bruno", "bruno@email.com", new DateTime(1990, 1, 1));
        _db.Add(customer!);

        var result = await _repository.GetByEmailAsync("bruno@email.com", TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Equal("Bruno", result.Data!.Name);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldFail_WhenNotFound()
    {
        var result = await _repository.GetByEmailAsync("notfound@email.com", TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnCorrectPage()
    {
        for (int i = 1; i <= 10; i++)
        {
            var (customer, _) = Customer.Create($"Customer {i}", $"c{i}@mail.com", new DateTime(1990, 1, 1));
            _db.Add(customer!);
        }

        var result = await _repository.GetPagedAsync(2, 3, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Equal(3, result.Items.Count());
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(4, result.TotalPages);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReplaceCustomer_WhenValid()
    {
        var (original, _) = Customer.Create("Lucas", "lucas@email.com", new DateTime(1990, 1, 1));
        _db.Add(original!);

        var updated = Customer.Create("Lucas Silva", "lucas@email.com", new DateTime(1990, 1, 1)).customer!;
        typeof(Customer).GetProperty("Id")!.SetValue(updated, original!.Id);

        var result = await _repository.UpdateAsync(updated, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.DoesNotContain(original, _db);
        Assert.Contains(updated, _db);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenCustomerIsInvalid()
    {
        var (original, _) = Customer.Create("Paula", "paula@email.com", new DateTime(1990, 1, 1));
        _db.Add(original!);

        var invalid = Customer.Create("", "", new DateTime(1990, 1, 1)).customer!;

        var result = await _repository.UpdateAsync(invalid, TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.Contains(original, _db);
        Assert.DoesNotContain(invalid, _db);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveCustomer_WhenExists()
    {
        var (customer, _) = Customer.Create("Rafael", "rafael@email.com", new DateTime(1990, 1, 1));
        _db.Add(customer!);

        var result = await _repository.DeleteAsync(customer!.Id, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.DoesNotContain(customer, _db);
    }

    [Fact]
    public async Task DeleteAsync_ShouldFail_WhenNotFound()
    {
        var result = await _repository.DeleteAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        Assert.False(result.Success);
    }
}
