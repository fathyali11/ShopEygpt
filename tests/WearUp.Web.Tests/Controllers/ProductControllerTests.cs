namespace WearUp.Web.Controllers.Tests;
public class ProductControllerTests
{
    private readonly ProductController _productController;
    private readonly Mock<IProductRepository> _productRepositoryMock;

    public ProductControllerTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _productController = new ProductController(_productRepositoryMock.Object);

        var httpContext = new DefaultHttpContext();
        var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        _productController.TempData = tempData;
        // Fake User
        var claim = new Claim(ClaimTypes.NameIdentifier, "user");
        var claimIdentity = new ClaimsIdentity([claim], "mock");
        var user = new ClaimsPrincipal(claimIdentity);
        _productController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task Index_WhenCallIt_ShouldReturnViewWithProducts()
    {
        // Arrange
        var productsForAdmin = new List<ProductReponseForAdmin>
        {
            new() { Id = 1, Name = "product1" },
            new() { Id = 2, Name = "product2" },
            new() { Id = 3, Name = "product3" },
            new() { Id = 4, Name = "product4" },
            new() { Id = 5, Name = "product5" }
        };

        var paginatedProducts = PaginatedList<ProductReponseForAdmin>.Create(productsForAdmin, 1, 10);
        var request = new FilterRequest(null!, null!, null!);
        _productRepositoryMock.Setup(x => x.GetAllProductsAdminAsync(It.IsAny<FilterRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedProducts);

        // Act
        var result = await _productController.Index(request);

        // Assert
        result.Should().NotBeNull();
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().NotBeNull();
        viewResult.Model.Should().BeEquivalentTo(paginatedProducts);
    }

    [Fact]
    public async Task LoadDiscover_WhenCallIt_ShouldReturnViewWithProducts()
    {
        // Arrange
        var products = new List<DiscoverProductVM>
        {
            new(1, "product1", "", "", "", 11, true),
            new(2, "product2", "", "", "", 11, true),
            new(3, "product3", "", "", "", 11, true),
            new(4, "product4", "", "", "", 11, true),
        };
        _productRepositoryMock.Setup(x => x.GetDiscoverProductsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _productController.LoadDiscover(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var partialViewResult = result.Should().BeOfType<PartialViewResult>().Subject;
        partialViewResult.Model.Should().NotBeNull();
        partialViewResult.Model.Should().Be(products);
        partialViewResult.ViewName.Should().Be("_DiscoverPartial");
    }

    [Fact]
    public async Task LoadNewArrivals_WhenCallIt_ShouldReturnViewWithProducts()
    {
        // Arrange
        var products = new List<NewArrivalProductsVM>
        {
            new(1, "product1", "", 11, true),
            new(2, "product2", "", 11, true),
            new(3, "product3", "", 11, true),
            new(4, "product4", "", 11, true),
        };
        _productRepositoryMock.Setup(x => x.GetNewArrivalProductsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _productController.LoadNewArrivals(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var partialViewResult = result.Should().BeOfType<PartialViewResult>().Subject;
        partialViewResult.Model.Should().NotBeNull();
        partialViewResult.Model.Should().Be(products);
        partialViewResult.ViewName.Should().Be("_NewArrivalPartial");
    }

    [Fact]
    public async Task LoadBestSellers_WhenCallIt_ShouldReturnViewWithProducts()
    {
        // Arrange
        var products = new List<BestSellingProductVM>
        {
            new(1, "product1", "", 11, true),
            new(2, "product2", "", 11, true),
            new(3, "product3", "", 11, true),
        };
        _productRepositoryMock.Setup(x => x.GetBestSellingProductsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);

        // Act
        var result = await _productController.LoadBestSellers(CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var partialViewResult = result.Should().BeOfType<PartialViewResult>().Subject;
        partialViewResult.Model.Should().NotBeNull();
        partialViewResult.Model.Should().Be(products);
        partialViewResult.ViewName.Should().Be("_BestSellersPartial");
    }

    [Fact]
    public async Task AllProductsBasedOnSort_WhenCallIt_ShouldReturnViewWithProducts()
    {
        // Arrange
        var products = new List<DiscoverProductVM>
        {
            new(1, "product1", "", "", "", 11, true),
            new(2, "product2", "", "", "", 11, true),
            new(3, "product3", "", "", "", 11, true),
            new(4, "product4", "", "", "", 11, true),
        };
        var paginatedProducts = PaginatedList<DiscoverProductVM>.Create(products, 1, 10);
        _productRepositoryMock.Setup(x => x.GetAllProductsSortedByAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedProducts);

        // Act
        var result = await _productController.AllProductsBasedOnSort("price", 1, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().NotBeNull();
        viewResult.Model.Should().BeEquivalentTo(paginatedProducts);
    }

    [Fact]
    public async Task Discover_WhenValidId_ShouldReturnViewWithProduct()
    {
        // Arrange
        var product = new DiscoverProductVM(1, "product1", "", "", "", 11, true);
        _productRepositoryMock.Setup(x => x.GetDiscoverProductByIdAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _productController.Discover(1, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().NotBeNull();
        viewResult.Model.Should().Be(product);
    }

    [Fact]
    public async Task GetAllInCategory_WhenCallIt_ShouldReturnViewWithProducts()
    {
        // Arrange
        var products = new List<DiscoverProductVM>
        {
            new DiscoverProductVM(1,"product1","","","",1,true),
            new DiscoverProductVM(2,"product2","","","",1,true),
            new DiscoverProductVM(3,"product3","","","",1,true),
        };
        var paginatedProducts = PaginatedList<DiscoverProductVM>.Create(products, 1, 10);
        _productRepositoryMock.Setup(x => x.GetAllProductsInCategoryAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedProducts);

        // Act
        var result = await _productController.GetAllInCategory(1, 1, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().NotBeNull();
        viewResult.Model.Should().BeEquivalentTo(paginatedProducts);
        viewResult.ViewName.Should().Be("AllProductsBasedOnSort");
    }

    [Fact]
    public async Task Search_WhenCallIt_ShouldReturnViewWithProducts()
    {
        // Arrange
        var products = new List<DiscoverProductVM>
        {
            new DiscoverProductVM(1,"product1","","","",1,true),
            new DiscoverProductVM(2,"product2","","","",1,true),
            new DiscoverProductVM(3,"product3","","","",1,true),
        };
        var paginatedProducts = PaginatedList<DiscoverProductVM>.Create(products, 1, 10);
        _productRepositoryMock.Setup(x => x.SearchInProductsInHomeAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedProducts);

        // Act
        var result = await _productController.Search("test", CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().NotBeNull();
        viewResult.Model.Should().BeEquivalentTo(paginatedProducts);
        viewResult.ViewName.Should().Be("AllProductsBasedOnSort");
    }

    [Fact]
    public void Create_WhenGetRequest_ShouldReturnView()
    {
        // Act
        var result = _productController.Create();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ViewResult>();
    }

    [Fact]
    public async Task Create_WhenPostValidModel_ShouldRedirectToIndex()
    {
        // Arrange
        var model = new CreateProductVM("product1","",1,1,11,new Mock<IFormFile>().Object);
        _productRepositoryMock.Setup(x => x.AddProductAsync(It.IsAny<CreateProductVM>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _productController.Create(model, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("Index");
        _productController.TempData["SuccessMessage"].Should().Be("Data Created Successfully");
    }

    [Fact]
    public async Task Create_WhenPostInvalidModel_ShouldReturnViewWithErrors()
    {
        // Arrange
        var model = new CreateProductVM(default, default, default, default, default, default);
        var errors = new List<ValidationError> { new ValidationError("Name", "Name is required") };
        _productRepositoryMock.Setup(x => x.AddProductAsync(It.IsAny<CreateProductVM>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(errors);
       

        // Act
        var result = await _productController.Create(model, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().Be(model);
        _productController.TempData["ErrorMessage"].Should().Be("Data Not Created");
    }

    [Fact]
    public async Task Edit_WhenValidId_ShouldReturnViewWithProduct()
    {
        // Arrange
        var product = new EditProductVM { Id = 1, Name = "product1" };
        _productRepositoryMock.Setup(x => x.GetProductEditByIdAsync(It.IsAny<int>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _productController.Edit(1);

        // Assert
        result.Should().NotBeNull();
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().NotBeNull();
        viewResult.Model.Should().Be(product);
    }

    [Fact]
    public async Task Edit_WhenInvalidId_ShouldRedirectToIndex()
    {
        // Arrange
        _productRepositoryMock.Setup(x => x.GetProductEditByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((EditProductVM)null!);

        // Act
        var result = await _productController.Edit(1);

        // Assert
        result.Should().NotBeNull();
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("Index");
        _productController.TempData["ErrorMessage"].Should().Be("Data Not Updated");
    }

    [Fact]
    public async Task EditAsync_WhenValidModel_ShouldRedirectToIndex()
    {
        // Arrange
        var model = new EditProductVM { Id = 1, Name = "product1" };
        _productRepositoryMock.Setup(x => x.UpdateProductAsync(It.IsAny<EditProductVM>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _productController.EditAsync(model, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("Index");
        _productController.TempData["SuccessMessage"].Should().Be("Data Updated Successfully");
    }

    [Fact]
    public async Task EditAsync_WhenInvalidModel_ShouldReturnViewWithErrors()
    {
        // Arrange
        var model = new EditProductVM();
        var errors = new List<ValidationError> { new ValidationError("Name", "Name is required") };
        _productRepositoryMock.Setup(x => x.UpdateProductAsync(It.IsAny<EditProductVM>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(errors);

        // Act
        var result = await _productController.EditAsync(model, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().Be(model);
        _productController.TempData["ErrorMessage"].Should().Be("Data Not Updated");
    }

    [Fact]
    public async Task Delete_WhenValidId_ShouldReturnSuccessJson()
    {
        // Arrange
        _productRepositoryMock.Setup(x => x.DeleteProductAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _productController.Delete(1);

        // Assert
        result.Should().NotBeNull();
        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        jsonResult.Value.Should().BeEquivalentTo(new { success = true, message = "Data Deleted Successfully" });
    }

    [Fact]
    public async Task Delete_WhenInvalidId_ShouldReturnFailureJson()
    {
        // Arrange
        _productRepositoryMock.Setup(x => x.DeleteProductAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _productController.Delete(1);

        // Assert
        result.Should().NotBeNull();
        var jsonResult = result.Should().BeOfType<JsonResult>().Subject;
        jsonResult.Value.Should().BeEquivalentTo(new { success = false, message = "Data Not Deleted" });
    }

    [Fact]
    public async Task Details_WhenValidId_ShouldReturnViewWithProduct()
    {
        // Arrange
        var product = new ProductReponseForAdmin { Id = 1, Name = "product1" };
        _productRepositoryMock.Setup(x => x.GetProductDetailsByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _productController.Details(1, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var viewResult = result.Should().BeOfType<ViewResult>().Subject;
        viewResult.Model.Should().NotBeNull();
        viewResult.Model.Should().Be(product);
    }

    [Fact]
    public async Task Details_WhenInvalidId_ShouldRedirectToIndex()
    {
        // Arrange
        _productRepositoryMock.Setup(x => x.GetProductDetailsByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ProductReponseForAdmin)null!);

        // Act
        var result = await _productController.Details(1, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var redirectResult = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirectResult.ActionName.Should().Be("Index");
        _productController.TempData["ErrorMessage"].Should().Be("Data Not Found");
    }
}