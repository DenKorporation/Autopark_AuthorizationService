using System.Net;
using Authorization.IntegrationTests.RestApis.Interfaces;
using AuthorizationService.BLL.DTOs.Response;
using AuthorizationService.DAL.Constants;
using FluentAssertions;
using Refit;

namespace Authorization.IntegrationTests.Controllers;

public class RoleControllerTests : BaseIntegrationTest
{
    private readonly IRolesApi _rolesApi;

    public RoleControllerTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
        _rolesApi = RestService.For<IRolesApi>(Client);
    }

    [Fact]
    public async Task GetAllRoles_RolesExist_ReturnsAllRoles()
    {
        List<RoleResponse> expectedCollection =
        [
            new RoleResponse(Roles.Administrator),
            new RoleResponse(Roles.Driver),
            new RoleResponse(Roles.Technician),
            new RoleResponse(Roles.InsuranceAgent),
            new RoleResponse(Roles.FleetManager),
            new RoleResponse(Roles.HrManager),
        ];

        var response = await _rolesApi.GetAllRolesAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var roleResponse = response.Content!.ToList();

        roleResponse.Should().NotBeNull();
        roleResponse.Should().Contain(expectedCollection);
    }
}
