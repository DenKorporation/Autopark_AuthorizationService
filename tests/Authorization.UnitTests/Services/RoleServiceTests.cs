using Authorization.UnitTests.DataGenerators;
using AuthorizationService.BLL.MappingProfiles;
using AuthorizationService.BLL.Services.Implementations;
using AuthorizationService.DAL.Repositories.Interfaces;
using AutoMapper;
using FluentAssertions;
using MockQueryable;
using Moq;

namespace Authorization.UnitTests.Services;

public class RoleServiceTests
{
    private readonly IMapper _mapper =
        new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<RoleProfile>()));

    private readonly Mock<IRoleRepository> _mockRoleRepository = new Mock<IRoleRepository>();
    private readonly RoleService _roleService;

    public RoleServiceTests()
    {
        _roleService = new RoleService(
            _mockRoleRepository.Object,
            _mapper);
    }

    [Fact]
    public async Task GetAllAsync_RolesExist_ReturnsList()
    {
        // Arrange
        var roleFaker = DataFakers
            .RoleFaker
            .Generate(5)
            .AsQueryable()
            .BuildMock();

        _mockRoleRepository
            .Setup(repo => repo.GetAllRolesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(roleFaker);

        // Act
        var result = await _roleService.GetAllRolesAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Count().Should().Be(5);
    }
}
