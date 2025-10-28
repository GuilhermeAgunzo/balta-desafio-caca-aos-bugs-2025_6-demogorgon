using BugStore.Domain.Entities;
using BugStore.Infrastructure.Tests.Repositories;

namespace BugStore.Infrastructure.Tests.UnitTests;

public class ProductUnitTest
{
    private readonly List<Product> _db = [];
    private readonly FakeProductRepository _repository;

    public ProductUnitTest()
    {
        _repository = new FakeProductRepository(_db);
    }

    [Fact]
    public async Task AddAsync_ShouldAddValidProduct()
    {
        var (product, _) = Product.Create("Mouse", "Wireless mouse", 99.90m);

        var result = await _repository.AddAsync(product!, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Contains(product, _db);
    }

    [Fact]
    public async Task AddAsync_ShouldFail_WhenProductIsInvalid()
    {
        var (product, _) = Product.Create("", "", -1);

        var result = await _repository.AddAsync(product!, TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.DoesNotContain(product, _db);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenExists()
    {
        var (product, _) = Product.Create("Keyboard", "Mechanical keyboard", 199.90m);
        _db.Add(product!);

        var result = await _repository.GetByIdAsync(product!.Id, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Equal(product.Id, result.Data!.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldFail_WhenNotFound()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetBySlugAsync_ShouldReturnProduct_WhenExists()
    {
        var (product, _) = Product.Create("Monitor", "4K monitor", 999.90m);
        _db.Add(product!);

        var result = await _repository.GetBySlugAsync(product!.Slug, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Equal(product.Slug, result.Data!.Slug);
    }

    [Fact]
    public async Task GetBySlugAsync_ShouldFail_WhenNotFound()
    {
        var result = await _repository.GetBySlugAsync("non-existent-slug", TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.Null(result.Data);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnCorrectPage()
    {
        for (int i = 1; i <= 10; i++)
        {
            var (product, _) = Product.Create($"Product {i}", "Test", 10m * i);
            _db.Add(product!);
        }

        var result = await _repository.GetPagedAsync(2, 3, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.Equal(3, result.Items.Count());
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(4, result.TotalPages);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReplaceProduct_WhenValid()
    {
        var (original, _) = Product.Create("Tablet", "Android tablet", 499.90m);
        _db.Add(original!);

        var updated = Product.Create("Tablet Pro", "Updated tablet", 599.90m, original!.Slug).product!;
        typeof(Product).GetProperty("Id")!.SetValue(updated, original.Id);

        var result = await _repository.UpdateAsync(updated, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.DoesNotContain(original, _db);
        Assert.Contains(updated, _db);
    }

    [Fact]
    public async Task UpdateAsync_ShouldFail_WhenProductIsInvalid()
    {
        var (original, _) = Product.Create("Speaker", "Bluetooth speaker", 149.90m);
        _db.Add(original!);

        var invalid = Product.Create("", "", -10).product!;

        var result = await _repository.UpdateAsync(invalid, TestContext.Current.CancellationToken);

        Assert.False(result.Success);
        Assert.Contains(original, _db);
        Assert.DoesNotContain(invalid, _db);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProduct_WhenExists()
    {
        var (product, _) = Product.Create("Camera", "Digital camera", 299.90m);
        _db.Add(product!);

        var result = await _repository.DeleteAsync(product!.Id, TestContext.Current.CancellationToken);

        Assert.True(result.Success);
        Assert.DoesNotContain(product, _db);
    }

    [Fact]
    public async Task DeleteAsync_ShouldFail_WhenNotFound()
    {
        var result = await _repository.DeleteAsync(Guid.NewGuid(), TestContext.Current.CancellationToken);

        Assert.False(result.Success);
    }
}