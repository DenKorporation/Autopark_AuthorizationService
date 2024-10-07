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

public class PassportServiceTests
{
    private readonly IMapper _mapper =
        new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<PassportProfile>()));

    private readonly Mock<IPassportRepository> _mockPassportRepository = new Mock<IPassportRepository>();
    private readonly Mock<IUserRepository> _mockUserRepository = new Mock<IUserRepository>();
    private readonly Mock<IUserClaimRepository> _mockUserClaimRepository = new Mock<IUserClaimRepository>();
    private readonly PassportService _passportService;

    public PassportServiceTests()
    {
        _passportService = new PassportService(
            _mockPassportRepository.Object,
            _mockUserRepository.Object,
            _mockUserClaimRepository.Object,
            _mapper);
    }

    [Fact]
    public async Task GetAllAsync_PassportExist_ReturnsPagedList()
    {
        // Arrange
        var passportFaker = DataFakers
            .PassportFaker
            .Generate(5)
            .AsQueryable()
            .BuildMock();

        _mockPassportRepository
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(passportFaker);

        var request = new GetAllRequest(1, 10);

        // Act
        var result = await _passportService.GetAllAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Count.Should().Be(5);
    }

    [Fact]
    public async Task GetByIdAsync_PassportExist_ReturnsOkWithPassport()
    {
        // Arrange
        var passport = DataFakers.PassportFaker.Generate();
        _mockPassportRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(passport);

        // Act
        var result = await _passportService.GetByIdAsync(passport.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(passport.Id);
        _mockPassportRepository.Verify(
            x => x.GetByIdAsync(
                passport.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_PassportNotExist_ReturnsPassportNotFoundError()
    {
        var id = Guid.NewGuid();

        // Arrange
        _mockPassportRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Passport?)null);

        // Act
        var result = await _passportService.GetByIdAsync(id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<PassportNotFoundError>();
        _mockPassportRepository.Verify(
            x => x.GetByIdAsync(
                id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SaveChangesFailure_ReturnsInternalServerError()
    {
        // Arrange
        var user = DataFakers.UserFaker.Generate();
        var passportRequest = DataFakers
            .PassportRequestFaker
            .RuleFor(p => p.UserId, _ => user.Id)
            .Generate();

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockPassportRepository
            .Setup(x => x.GetByIdentificationNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Passport?)null);

        _mockPassportRepository
            .Setup(
                x => x.GetBySeriesAndNumberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Passport?)null);

        _mockPassportRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Passport>().BuildMock());

        _mockPassportRepository
            .Setup(x => x.CreateAsync(It.IsAny<Passport>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _passportService.CreateAsync(passportRequest);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _mockPassportRepository.Verify(
            x => x.CreateAsync(
                It.IsAny<Passport>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SaveChangesSuccess_ReturnsOkWithValue()
    {
        // Arrange
        var user = DataFakers.UserFaker.Generate();
        var passportRequest = DataFakers
            .PassportRequestFaker
            .RuleFor(p => p.UserId, _ => user.Id)
            .Generate();

        var passport = DataFakers
            .PassportFaker
            .RuleFor(p => p.UserId, _ => user.Id)
            .Generate();

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockPassportRepository
            .Setup(x => x.GetByIdentificationNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Passport?)null);

        _mockPassportRepository
            .Setup(
                x => x.GetBySeriesAndNumberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Passport?)null);

        _mockPassportRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Passport>().BuildMock());

        _mockPassportRepository
            .Setup(x => x.CreateAsync(It.IsAny<Passport>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(passport);

        _mockUserClaimRepository
            .Setup(
                x => x.AddClaimsAsync(It.IsAny<User>(), It.IsAny<IEnumerable<Claim>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _passportService.CreateAsync(passportRequest);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _mockPassportRepository.Verify(
            x => x.CreateAsync(
                It.IsAny<Passport>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_PassportNotExist_ReturnsPassportNotFoundError()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockPassportRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Passport?)null);

        // Act
        var result = await _passportService.UpdateAsync(id, null!);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<PassportNotFoundError>();
        _mockPassportRepository.Verify(
            x => x.GetByIdAsync(
                id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SaveChangesFailure_ReturnsInternalServerError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = DataFakers.UserFaker.Generate();
        var passportRequest = DataFakers
            .PassportRequestFaker
            .RuleFor(p => p.UserId, _ => user.Id)
            .Generate();

        var passport = DataFakers
            .PassportFaker
            .RuleFor(p => p.UserId, _ => user.Id)
            .Generate();

        _mockPassportRepository
            .Setup(x => x.GetByIdentificationNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Passport?)null);

        _mockPassportRepository
            .Setup(
                x => x.GetBySeriesAndNumberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Passport?)null);

        _mockPassportRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Passport>().BuildMock());

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockPassportRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(passport);

        _mockPassportRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Passport>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _passportService.UpdateAsync(id, passportRequest);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _mockPassportRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Passport>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SaveChangesSuccess_ReturnsOkWithValue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = DataFakers.UserFaker.Generate();
        var passportRequest = DataFakers
            .PassportRequestFaker
            .RuleFor(p => p.UserId, _ => user.Id)
            .Generate();

        var passport = DataFakers
            .PassportFaker
            .RuleFor(p => p.UserId, _ => user.Id)
            .Generate();

        _mockPassportRepository
            .Setup(x => x.GetByIdentificationNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Passport?)null);

        _mockPassportRepository
            .Setup(
                x => x.GetBySeriesAndNumberAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Passport?)null);

        _mockPassportRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Passport>().BuildMock());

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockPassportRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(passport);

        _mockPassportRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Passport>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(passport);

        _mockUserClaimRepository
            .Setup(
                x => x.UpdateClaimsAsync(
                    It.IsAny<User>(),
                    It.IsAny<IEnumerable<Claim>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _passportService.UpdateAsync(id, passportRequest);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _mockPassportRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Passport>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_PassportNotExist_ReturnsPassportNotFoundError()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockPassportRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Passport?)null);

        // Act
        var result = await _passportService.DeleteAsync(id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<PassportNotFoundError>();
        _mockPassportRepository.Verify(
            x => x.GetByIdAsync(
                id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_SaveChangesFailure_ReturnsInternalServerError()
    {
        // Arrange
        var user = DataFakers.UserFaker.Generate();
        var passport = DataFakers
            .PassportFaker
            .RuleFor(p => p.UserId, _ => user.Id)
            .Generate();

        _mockPassportRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(passport);

        _mockPassportRepository
            .Setup(x => x.DeleteAsync(It.IsAny<Passport>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _passportService.DeleteAsync(passport.Id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _mockPassportRepository.Verify(
            x => x.DeleteAsync(
                It.IsAny<Passport>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_SaveChangesSuccess_ReturnsOk()
    {
        // Arrange
        var user = DataFakers.UserFaker.Generate();
        var passport = DataFakers
            .PassportFaker
            .RuleFor(p => p.UserId, _ => user.Id)
            .Generate();

        _mockPassportRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(passport);

        // Act
        var result = await _passportService.DeleteAsync(passport.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _mockPassportRepository.Verify(
            x => x.DeleteAsync(
                It.IsAny<Passport>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
