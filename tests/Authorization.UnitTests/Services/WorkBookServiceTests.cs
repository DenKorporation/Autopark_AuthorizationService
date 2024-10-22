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

public class WorkBookServiceTests
{
    private readonly IMapper _mapper =
        new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<WorkBookProfile>()));

    private readonly Mock<IWorkBookRepository> _mockWorkBookRepository = new Mock<IWorkBookRepository>();
    private readonly Mock<IUserRepository> _mockUserRepository = new Mock<IUserRepository>();
    private readonly WorkBookService _workBookService;

    public WorkBookServiceTests()
    {
        _workBookService = new WorkBookService(
            _mockWorkBookRepository.Object,
            _mockUserRepository.Object,
            _mapper);
    }

    [Fact]
    public async Task GetAllAsync_WorkBooksExist_ReturnsPagedList()
    {
        // Arrange
        var workBookFaker = DataFakers
            .WorkBookFaker
            .Generate(5)
            .AsQueryable()
            .BuildMock();

        _mockWorkBookRepository
            .Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(workBookFaker);

        var request = new GetAllRequest(1, 10);

        // Act
        var result = await _workBookService.GetAllAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Count.Should().Be(5);
    }

    [Fact]
    public async Task GetByIdAsync_WorkBookExist_ReturnsOkWithWorkBook()
    {
        // Arrange
        var workBook = DataFakers.WorkBookFaker.Generate();
        _mockWorkBookRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(workBook);

        // Act
        var result = await _workBookService.GetByIdAsync(workBook.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(workBook.Id);
        _mockWorkBookRepository.Verify(
            x => x.GetByIdAsync(
                workBook.Id,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WorkBookNotExist_ReturnsWorkBookNotFoundError()
    {
        var id = Guid.NewGuid();

        // Arrange
        _mockWorkBookRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkBook?)null);

        // Act
        var result = await _workBookService.GetByIdAsync(id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<WorkBookNotFoundError>();
        _mockWorkBookRepository.Verify(
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
        var workBookRequest = DataFakers
            .WorkBookRequestFaker
            .RuleFor(wb => wb.UserId, _ => user.Id)
            .Generate();

        _mockWorkBookRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<WorkBook>().BuildMock());

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockWorkBookRepository
            .Setup(x => x.CreateAsync(It.IsAny<WorkBook>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _workBookService.CreateAsync(workBookRequest);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _mockWorkBookRepository.Verify(
            x => x.CreateAsync(
                It.IsAny<WorkBook>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateAsync_SaveChangesSuccess_ReturnsOkWithValue()
    {
        // Arrange
        var user = DataFakers.UserFaker.Generate();
        var workBookRequest = DataFakers
            .WorkBookRequestFaker
            .RuleFor(wb => wb.UserId, _ => user.Id)
            .Generate();

        var workBook = DataFakers
            .WorkBookFaker
            .RuleFor(wb => wb.UserId, _ => user.Id)
            .Generate();

        _mockWorkBookRepository
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<WorkBook>().BuildMock());

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mockWorkBookRepository
            .Setup(x => x.CreateAsync(It.IsAny<WorkBook>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(workBook);

        // Act
        var result = await _workBookService.CreateAsync(workBookRequest);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _mockWorkBookRepository.Verify(
            x => x.CreateAsync(
                It.IsAny<WorkBook>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WorkBookNotExist_ReturnsWorkBookNotFoundError()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockWorkBookRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkBook?)null);

        // Act
        var result = await _workBookService.UpdateAsync(id, null!);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<WorkBookNotFoundError>();
        _mockWorkBookRepository.Verify(
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
        var workBookRequest = DataFakers
            .WorkBookRequestFaker
            .RuleFor(wb => wb.UserId, _ => user.Id)
            .Generate();

        var workBook = DataFakers
            .WorkBookFaker
            .RuleFor(wb => wb.UserId, _ => user.Id)
            .Generate();

        _mockWorkBookRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(workBook);

        _mockWorkBookRepository
            .Setup(x => x.UpdateAsync(It.IsAny<WorkBook>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _workBookService.UpdateAsync(id, workBookRequest);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _mockWorkBookRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<WorkBook>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_SaveChangesSuccess_ReturnsOkWithValue()
    {
        // Arrange
        var id = Guid.NewGuid();
        var user = DataFakers.UserFaker.Generate();
        var workBookRequest = DataFakers
            .WorkBookRequestFaker
            .RuleFor(wb => wb.UserId, _ => user.Id)
            .Generate();

        var workBook = DataFakers
            .WorkBookFaker
            .RuleFor(wb => wb.UserId, _ => user.Id)
            .Generate();

        _mockWorkBookRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(workBook);

        _mockWorkBookRepository
            .Setup(x => x.UpdateAsync(It.IsAny<WorkBook>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(workBook);

        // Act
        var result = await _workBookService.UpdateAsync(id, workBookRequest);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _mockWorkBookRepository.Verify(
            x => x.UpdateAsync(
                It.IsAny<WorkBook>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WorkBookNotExist_ReturnsWorkBookNotFoundError()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockWorkBookRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkBook?)null);

        // Act
        var result = await _workBookService.DeleteAsync(id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<WorkBookNotFoundError>();
        _mockWorkBookRepository.Verify(
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
        var workBook = DataFakers
            .WorkBookFaker
            .RuleFor(wb => wb.UserId, _ => user.Id)
            .Generate();

        _mockWorkBookRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(workBook);

        _mockWorkBookRepository
            .Setup(x => x.DeleteAsync(It.IsAny<WorkBook>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _workBookService.DeleteAsync(workBook.Id);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainItemsAssignableTo<InternalServerError>();
        _mockWorkBookRepository.Verify(
            x => x.DeleteAsync(
                It.IsAny<WorkBook>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_SaveChangesSuccess_ReturnsOk()
    {
        // Arrange
        var user = DataFakers.UserFaker.Generate();
        var workBook = DataFakers
            .WorkBookFaker
            .RuleFor(wb => wb.UserId, _ => user.Id)
            .Generate();

        _mockWorkBookRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(workBook);

        // Act
        var result = await _workBookService.DeleteAsync(workBook.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Errors.Should().BeEmpty();
        _mockWorkBookRepository.Verify(
            x => x.DeleteAsync(
                It.IsAny<WorkBook>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
