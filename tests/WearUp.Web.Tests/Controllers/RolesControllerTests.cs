using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;
using System.Security.Claims;
using Web.Entites.IRepositories;
using Xunit;

namespace WearUp.Web.Controllers.Tests;

public class RolesControllerTests
{
    private RolesController _rolesController;
    private Mock<IRoleRepository> _rolesRepositoryMock; 
    public RolesControllerTests()
    {
        _rolesRepositoryMock = new Mock<IRoleRepository>();
        _rolesController=new RolesController(_rolesRepositoryMock.Object);

        // Fake User
        var claim = new Claim(ClaimTypes.NameIdentifier, "user");
        var claimIdentity = new ClaimsIdentity([claim], "mock");
        var user = new ClaimsPrincipal(claimIdentity);
        _rolesController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }
    [Fact()]
    public async Task LoadRolesSelectListTest()
    {
        // arrange
        var roles = new List<SelectListItem>
        {
            new SelectListItem{Text="role1",Value="1"},
            new SelectListItem{Text="role2",Value="2"}
        };

        _rolesRepositoryMock.Setup(x=>x.GetRoleSelectListAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);

        // act
        var result=await _rolesController.LoadRolesSelectList();

        // assert
        result.Should().NotBeNull();
        var partialViewModel = result.Should().BeOfType<PartialViewResult>().Subject;
        partialViewModel.Should().NotBeNull();
        partialViewModel.Model.Should().NotBeNull();
        partialViewModel.Model.Should().BeEquivalentTo(roles);
        partialViewModel.ViewName.Should().Be("_RolesSelectListPartial");

    }
}