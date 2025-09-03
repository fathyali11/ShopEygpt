using FluentAssertions;
using Mapster;
using Microsoft.AspNetCore.Http;
using Moq;
using Web.Entites.Models;
using Web.Entites.ViewModels.ProductVMs;
using Xunit;

namespace Web.Entites.Mappings.Tests;

public class ProductMappingTests
{
    [Fact]
    public void Register_WhenMapCreateProductVMToProduct_ShouldReturnProduct()
    {
        var config = new TypeAdapterConfig();
        var mapping = new ProductMapping();
        mapping.Register(config);
        var createProductVM = new CreateProductVM
        (
            "Smartphone",
            "A latest model smartphone",
            800.00m,
            1,
            100,
            new Mock<IFormFile>().Object
        );
        // Act
        var product = createProductVM.Adapt<Product>(config);
        // Assert
        product.Should().NotBeNull();
        product.Name.Should().Be(createProductVM.Name);
        product.Description.Should().Be(createProductVM.Description);
        product.Price.Should().Be(createProductVM.Price);
        product.CategoryId.Should().Be(createProductVM.CategoryId);
    }

    [Fact]
    public void Register_WhenMapProductToProductReponseForAdminWhenCategoryNameFound_ShouldReturnProductReponseForAdmin()
    {
        var config = new TypeAdapterConfig();
        var mapping = new ProductMapping();
        mapping.Register(config);
        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Description = "A high-end laptop",
            Price = 1500.00m,
            ImageName = "laptop.jpg",
            CategoryId = 2, 
            Category = new Models.Category { Id = 2, Name = "Computers", ImageName = "computers.jpg" }
        };
        // Act
        var productResponseVM = product.Adapt<ProductReponseForAdmin>(config);
        // Assert
        productResponseVM.Should().NotBeNull();
        productResponseVM.Id.Should().Be(product.Id);
        productResponseVM.Name.Should().Be(product.Name);
        productResponseVM.Description.Should().Be(product.Description);
        productResponseVM.Price.Should().Be(product.Price);
        productResponseVM.ImageName.Should().Be(product.ImageName);
        productResponseVM.CategoryName.Should().Be(product.Category.Name);
    }

    [Fact]
    public void Register_WhenMapProductToProductReponseForAdminWhenCategoryNameNotFound_ShouldReturnProductReponseForAdminWithCategoryNameEmpty()
    {
        var config = new TypeAdapterConfig();
        var mapping = new ProductMapping();
        mapping.Register(config);
        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Description = "A high-end laptop",
            Price = 1500.00m,
            ImageName = "laptop.jpg",
            CategoryId = 2,
            Category = null!
        };
        // Act
        var productResponseVM = product.Adapt<ProductReponseForAdmin>(config);
        // Assert
        productResponseVM.Should().NotBeNull();
        productResponseVM.Id.Should().Be(product.Id);
        productResponseVM.Name.Should().Be(product.Name);
        productResponseVM.Description.Should().Be(product.Description);
        productResponseVM.Price.Should().Be(product.Price);
        productResponseVM.ImageName.Should().Be(product.ImageName);
        productResponseVM.CategoryName.Should().Be(string.Empty);
    }
    [Fact]
    public void Register_WhenMapProductToDiscoverProductVM_ShouldReturnDiscoverProductVM()
    {
        var config = new TypeAdapterConfig();
        var mapping = new ProductMapping();
        mapping.Register(config);
        var product = new Product
        {
            Id = 1,
            Name = "Laptop",
            Description = "A high-end laptop",
            Price = 1500.00m,
            ImageName = "laptop.jpg",
            CategoryId = 2,
            Category = new Category { Id = 2, Name = "Computers", ImageName = "computers.jpg" }
        };
        // Act
        var productResponseVM = product.Adapt<DiscoverProductVM>(config);
        // Assert
        productResponseVM.Should().NotBeNull();
        productResponseVM.Id.Should().Be(product.Id);
        productResponseVM.Name.Should().Be(product.Name);
        productResponseVM.Description.Should().Be(product.Description);
        productResponseVM.Price.Should().Be((double)product.Price);
        productResponseVM.ImageName.Should().Be(product.ImageName);
        productResponseVM.CategoryName.Should().Be(product.Category.Name);
    }

    [Fact]
    public void Register_WhenMapProductToEditProductVM_ShouldReturnEditProductVM()
    {
        var config = new TypeAdapterConfig();
        var mapping = new ProductMapping();
        mapping.Register(config);
        var product = new Models.Product
        {
            Id = 1,
            Name = "Tablet",
            Description = "A powerful tablet",
            Price = 600.00m,
            ImageName = "tablet.jpg",
            CategoryId = 3,
            Category = new Models.Category { Id = 3, Name = "Gadgets", ImageName = "gadgets.jpg" }
        };
        // Act
        var editProductVM = product.Adapt<EditProductVM>(config);
        // Assert
        editProductVM.Should().NotBeNull();
        editProductVM.Id.Should().Be(product.Id);
        editProductVM.Name.Should().Be(product.Name);
        editProductVM.Description.Should().Be(product.Description);
        editProductVM.Price.Should().Be(product.Price);
        editProductVM.ImageName.Should().Be(product.ImageName);
        editProductVM.CategoryId.Should().Be(product.CategoryId);
        editProductVM.CategoryName.Should().Be(product.Category.Name);
    }

    [Fact]
    public void Register_WhenMapEditProductVMToProduct_ShouldReturnProduct()
    {
        var config = new TypeAdapterConfig();
        var mapping = new ProductMapping();
        mapping.Register(config);
        var editProductVM = new EditProductVM
        {
            Id = 1,
            Name = "Tablet",
            Description = "A powerful tablet",
            Price = 600.00m,
            ImageName = "tablet.jpg",
            CategoryId = 3,
            CategoryName = "Gadgets"
        };
        // Act
        var product = editProductVM.Adapt<Product>(config);
        // Assert
        product.Should().NotBeNull();
        product.Id.Should().Be(editProductVM.Id);
        product.Name.Should().Be(editProductVM.Name);
        product.Description.Should().Be(editProductVM.Description);
        product.Price.Should().Be(editProductVM.Price);
        product.CategoryId.Should().Be(editProductVM.CategoryId);
        product.ImageName.Should().BeNullOrEmpty(); 
        product.Category.Should().BeNull();
    }


}