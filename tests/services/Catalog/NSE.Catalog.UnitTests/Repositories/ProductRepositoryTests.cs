using FluentAssertions;
using NSE.Catalog.API.Data.Repository;
using NSE.Catalog.API.Models.Entities;
using NSE.Catalog.UnitTests.Factories;

namespace NSE.Catalog.UnitTests.Repositories;

public class ProductRepositoryTests
{
    [Fact(DisplayName = "Deve obter o produto pelo id")]
    public async Task Should_Get_Product_By_Id()
    {
        // Arrange
        var context = CatalogContextFactory.Create();

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Notebook Gamer",
            Active = true,
            Description = "Notebook Gamer Description",
            Price = 13.5M,
            DateAdded = DateTime.Now,
            Image = "notebook-gamer.png",
            Stock = 5
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetById(product.Id);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Notebook Gamer");
    }

    [Fact(DisplayName = "Deve retornar apenas produtos ativos")]
    public async Task Should_Return_Only_Active_Products()
    {
        // Arrange
        var context = CatalogContextFactory.Create();

        var activeProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Mouse",
            Active = true,
            Description = "Mouse Description",
            Price = 13.5M,
            DateAdded = DateTime.Now,
            Image = "mouse.png",
            Stock = 5
        };

        var inactiveProduct = new Product
        {
            Id = Guid.NewGuid(),
            Name = "Keyboard",
            Active = false,
            Description = "Keyboard Description",
            Price = 13.5M,
            DateAdded = DateTime.Now,
            Image = "keyboard.png",
            Stock = 5
        };

        context.Products.AddRange(activeProduct, inactiveProduct);

        await context.SaveChangesAsync();

        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetProductsById(
            $"{activeProduct.Id},{inactiveProduct.Id}"
        );

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Mouse");
    }

    [Fact(DisplayName = "Deve retornar uma lista vazia quando o GUID for inválido")]
    public async Task Should_Return_Empty_List_When_Invalid_Guid()
    {
        // Arrange
        var context = CatalogContextFactory.Create();
        var repository = new ProductRepository(context);

        // Act
        var result = await repository.GetProductsById("invalid-guid");

        // Assert
        result.Should().BeEmpty();
    }
}