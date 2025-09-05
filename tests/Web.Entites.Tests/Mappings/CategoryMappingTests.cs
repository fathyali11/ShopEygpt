namespace Web.Entites.Mappings.Tests;

public class CategoryMappingTests
{
    [Fact()]
    public void Register_WhenMapCreateCategoryVMToCategory_ShouldReturnCategory()
    {
        var config = new TypeAdapterConfig();
        var mapping = new CategoryMapping();
        mapping.Register(config);
        var createCategoryVM = new CreateCategoryVM
            (
                "Electronics",
                new Mock<IFormFile>().Object
            );

        // Act
        var category = createCategoryVM.Adapt<Category>(config);
        // Assert
        category.Should().NotBeNull();
        category.Name.Should().Be(createCategoryVM.Name);
    }
    [Fact()]
    public void Register_WhenMapCategoryToCategoryResponse_ShouldReturnCategoryResponse()
    {
        var config = new TypeAdapterConfig();
        var mapping = new CategoryMapping();
        mapping.Register(config);
        var category = new Category
        {
            Id = 1,
            Name = "Electronics",
            ImageName = "electronics.jpg"
        };
        // Act
        var categoryResponse = category.Adapt<CategoryResponse>(config);
        // Assert
        categoryResponse.Should().NotBeNull();
        categoryResponse.Id.Should().Be(category.Id);
        categoryResponse.Name.Should().Be(category.Name);
        categoryResponse.ImageName.Should().Be(category.ImageName);
    }
    [Fact()]
    public void Register_WhenMapCategoryToEditCategoryVM_ShouldReturnEditCategoryVM()
    {
        var config = new TypeAdapterConfig();
        var mapping = new CategoryMapping();
        mapping.Register(config);
        var category = new Category
        {
            Id = 1,
            Name = "Electronics",
            ImageName = "electronics.jpg"
        };
        // Act
        var editCategoryVM = category.Adapt<EditCategoryVM>(config);
        // Assert
        editCategoryVM.Should().NotBeNull();
        editCategoryVM.Id.Should().Be(category.Id);
        editCategoryVM.Name.Should().Be(category.Name);
        editCategoryVM.ExistImageUrl.Should().Be(category.ImageName);
    }
}