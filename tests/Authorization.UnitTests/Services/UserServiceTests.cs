using System.Security.Claims;
using Authorization.UnitTests.DataGenerators;
using AuthorizationService.BLL.DTOs.Request;
using AuthorizationService.BLL.Errors;
using AuthorizationService.BLL.Errors.Base;
using AuthorizationService.BLL.MappingProfiles;
using AuthorizationService.BLL.Services.Implementations;
using AuthorizationService.DAL.Models;
using AuthorizationService.DAL.Repositories.Interfaces;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using MockQueryable;
using Moq;

namespace Authorization.UnitTests.Services;

public class UserServiceTests
{
    private readonly IMapper _mapper =
        new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<UserProfile>()));

    private readonly Mock<IUserRepository> _mockUserRepository = new Mock<IUserRepository>();
    private readonly Mock<IUserClaimRepository> _mockUserClaimsRepository = new Mock<IUserClaimRepository>();
    private readonly Mock<IRoleRepository> _mockRoleRepository = new Mock<IRoleRepository>();
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userService = new UserService(
            _mockUserRepository.Object,
            _mockRoleRepository.Object,
            _mockUserClaimsRepository.Object,
            _mapper);
    }

    [Fact]
    public async Task GetAllAsync_UsersExist_ReturnsPagedList()
    {
        // Arrange
        var userFaker = DataFakers
            .UserFaker
            .Generate(5)
            .AsQueryable()
            .BuildMock();

        _mockUserRepository
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(userFaker);

        var request = new UserFilterRequest(
            1,
            10,
            null,
            null,
            null);

        // Act
        var result = await _userService.GetAllAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Count.Should().Be(5);
    }

    [Fact]
    public async Task GetByIdAsync_UserExist_ReturnsOkWithUser()
    {
        // Arrange
        var user = DataFakers.UserFaker.Generate();
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetByIdAsync(user.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(user.Id);
        _mockUserRepository.Verify(
            x => x.GetByIdAsync(
                user.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_UserNotExist_ReturnsUserNotFoundError()
    {
        var id = Guid.NewGuid();

        // Arrange
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetByIdAsync(id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserNotFoundError>();
        _mockUserRepository.Verify(
            x => x.GetByIdAsync(
                id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByEmailAsync_UserExist_ReturnsOkWithUser()
    {
        // Arrange
        var user = DataFakers.UserFaker.Generate();
        _mockUserRepository.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetByEmailAsync(user.Email!);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(user.Id);
        _mockUserRepository.Verify(
            x => x.GetByEmailAsync(
                user.Email!,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByEmailAsync_UserNotExist_ReturnsUserNotFoundError()
    {
        const string email = "example@example.com";

        // Arrange
        _mockUserRepository.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetByEmailAsync(email);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserNotFoundError>();
        _mockUserRepository.Verify(
            x => x.GetByEmailAsync(
                email,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SaveChangesFailure_ReturnsInternalServerError()
    {
        // Arrange
        var userRequest = DataFakers
            .UserRequestFaker
            .Generate();

        _mockRoleRepository.Setup(x => x.RoleExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _mockUserRepository
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Failed());

        // Act
        var result = await _userService.CreateAsync(userRequest);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _mockUserRepository.Verify(
            x => x.CreateAsync(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SaveChangesSuccess_ReturnsOkWithValue()
    {
        // Arrange
        var user = DataFakers.UserFaker.Generate();
        var userRequest = DataFakers
            .UserRequestFaker
            .Generate();

        _mockRoleRepository.Setup(x => x.RoleExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _mockUserRepository
            .Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserRepository
            .Setup(x => x.AssignRoleAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserClaimsRepository
            .Setup(
                x => x.AddClaimsAsync(It.IsAny<User>(), It.IsAny<IEnumerable<Claim>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userService.CreateAsync(userRequest);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _mockUserRepository.Verify(
            x => x.CreateAsync(
                It.IsAny<User>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_UserNotExist_ReturnsUserNotFoundError()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.UpdateAsync(id, null!);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserNotFoundError>();
        _mockUserRepository.Verify(
            x => x.GetByIdAsync(
                id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SaveChangesFailure_ReturnsInternalServerError()
    {
        // Arrange
        var user = DataFakers.UserFaker.Generate();
        var userRequest = DataFakers
            .UserRequestFaker
            .Generate();

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _mockUserRepository
            .Setup(x => x.UpdateUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Failed());

        // Act
        var result = await _userService.UpdateAsync(user.Id, userRequest);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _mockUserRepository.Verify(
            x => x.UpdateUserAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SaveChangesSuccess_ReturnsOkWithValue()
    {
        // Arrange
        var user = DataFakers.UserFaker.Generate();
        var userRequest = DataFakers
            .UserRequestFaker
            .Generate();

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _mockUserRepository
            .Setup(x => x.UpdateUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserRepository
            .Setup(x => x.ChangePasswordAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserRepository
            .Setup(x => x.GetRoleAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userRequest.Role);

        _mockUserClaimsRepository
            .Setup(
                x => x.UpdateClaimsAsync(
                    It.IsAny<User>(),
                    It.IsAny<IEnumerable<Claim>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userService.UpdateAsync(user.Id, userRequest);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _mockUserRepository.Verify(
            x => x.UpdateUserAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_UserNotExist_ReturnsUserNotFoundError()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.DeleteAsync(id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<UserNotFoundError>();
        _mockUserRepository.Verify(
            x => x.GetByIdAsync(
                id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_SaveChangesFailure_ReturnsInternalServerError()
    {
        // Arrange
        var user = DataFakers
            .UserFaker
            .Generate();

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockUserRepository
            .Setup(x => x.DeleteUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _userService.DeleteAsync(user.Id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _mockUserRepository.Verify(
            x => x.DeleteUserAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_SaveChangesSuccess_ReturnsOk()
    {
        // Arrange
        var user = DataFakers
            .UserFaker
            .Generate();

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockUserRepository
            .Setup(x => x.DeleteUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _userService.DeleteAsync(user.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _mockUserRepository.Verify(
            x => x.DeleteUserAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
