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
using MockQueryable;
using Moq;

namespace Authorization.UnitTests.Services;

public class ContractServiceTests
{
    private readonly IMapper _mapper =
        new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<ContractProfile>()));

    private readonly Mock<IContractRepository> _mockContractRepository = new Mock<IContractRepository>();
    private readonly Mock<IUserRepository> _mockUserRepository = new Mock<IUserRepository>();
    private readonly ContractService _contractService;

    public ContractServiceTests()
    {
        _contractService = new ContractService(
            _mockContractRepository.Object,
            _mockUserRepository.Object,
            _mapper);
    }

    [Fact]
    public async Task GetAllAsync_ContractsExist_ReturnsPagedList()
    {
        // Arrange
        var contractFaker = DataFakers
            .ContractFaker
            .Generate(5)
            .AsQueryable()
            .BuildMock();

        _mockContractRepository
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(contractFaker);

        var request = new ContractFilterRequest(
            1,
            10,
            null,
            null,
            null,
            null,
            null);

        // Act
        var result = await _contractService.GetAllAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Count.Should().Be(5);
    }

    [Fact]
    public async Task GetByIdAsync_ContractExist_ReturnsOkWithContract()
    {
        // Arrange
        var contract = DataFakers.ContractFaker.Generate();
        _mockContractRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        // Act
        var result = await _contractService.GetByIdAsync(contract.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(contract.Id);
        _mockContractRepository.Verify(
            x => x.GetByIdAsync(
                contract.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_ContractNotExist_ReturnsContractNotFoundError()
    {
        var id = Guid.NewGuid();

        // Arrange
        _mockContractRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contract?)null);

        // Act
        var result = await _contractService.GetByIdAsync(id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<ContractNotFoundError>();
        _mockContractRepository.Verify(
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
        var contractRequest = DataFakers
            .ContractRequestFaker
            .RuleFor(c => c.UserId, _ => user.Id)
            .Generate();

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockContractRepository
            .Setup(x => x.CreateAsync(It.IsAny<Contract>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _contractService.CreateAsync(contractRequest);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _mockContractRepository.Verify(
            x => x.CreateAsync(
                It.IsAny<Contract>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SaveChangesSuccess_ReturnsOkWithValue()
    {
        // Arrange
        var user = DataFakers.UserFaker.Generate();
        var contractRequest = DataFakers
            .ContractRequestFaker
            .RuleFor(c => c.UserId, _ => user.Id)
            .Generate();

        var contract = DataFakers
            .ContractFaker
            .RuleFor(c => c.UserId, _ => user.Id)
            .Generate();

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockContractRepository
            .Setup(x => x.CreateAsync(It.IsAny<Contract>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        // Act
        var result = await _contractService.CreateAsync(contractRequest);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _mockContractRepository.Verify(
            x => x.CreateAsync(
                It.IsAny<Contract>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ContractNotExist_ReturnsContractNotFoundError()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockContractRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contract?)null);

        // Act
        var result = await _contractService.UpdateAsync(id, null!);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<ContractNotFoundError>();
        _mockContractRepository.Verify(
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
        var contractRequest = DataFakers
            .ContractRequestFaker
            .RuleFor(c => c.UserId, _ => user.Id)
            .Generate();

        var contract = DataFakers
            .ContractFaker
            .RuleFor(c => c.UserId, _ => user.Id)
            .Generate();

        _mockContractRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        _mockContractRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Contract>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _contractService.UpdateAsync(id, contractRequest);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _mockContractRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Contract>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SaveChangesSuccess_ReturnsOkWithValue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = DataFakers.UserFaker.Generate();
        var contractRequest = DataFakers
            .ContractRequestFaker
            .RuleFor(c => c.UserId, _ => user.Id)
            .Generate();

        var contract = DataFakers
            .ContractFaker
            .RuleFor(c => c.UserId, _ => user.Id)
            .Generate();

        _mockContractRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        _mockContractRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Contract>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        // Act
        var result = await _contractService.UpdateAsync(id, contractRequest);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _mockContractRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<Contract>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ContractNotExist_ReturnsContractNotFoundError()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockContractRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Contract?)null);

        // Act
        var result = await _contractService.DeleteAsync(id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<ContractNotFoundError>();
        _mockContractRepository.Verify(
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
        var contract = DataFakers
            .ContractFaker
            .RuleFor(c => c.UserId, _ => user.Id)
            .Generate();

        _mockContractRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        _mockContractRepository
            .Setup(x => x.DeleteAsync(It.IsAny<Contract>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _contractService.DeleteAsync(contract.Id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _mockContractRepository.Verify(
            x => x.DeleteAsync(
                It.IsAny<Contract>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_SaveChangesSuccess_ReturnsOk()
    {
        // Arrange
        var user = DataFakers.UserFaker.Generate();
        var contract = DataFakers
            .ContractFaker
            .RuleFor(c => c.UserId, _ => user.Id)
            .Generate();

        _mockContractRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contract);

        // Act
        var result = await _contractService.DeleteAsync(contract.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _mockContractRepository.Verify(
            x => x.DeleteAsync(
                It.IsAny<Contract>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
