using System.Net;
using Authorization.IntegrationTests.DataGenerators;
using Authorization.IntegrationTests.Responses;
using Authorization.IntegrationTests.RestApis.Interfaces;
using AuthorizationService.BLL.DTOs.Request;
using AuthorizationService.DAL.Models;
using FluentAssertions;
using Refit;

namespace Authorization.IntegrationTests.Controllers;

public class WorkBookControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly IWorkBooksApi _workBooksApi;
    private readonly Func<Task> _resetDatabase;

    public WorkBookControllerTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
        _workBooksApi = RestService.For<IWorkBooksApi>(Client);
        _resetDatabase = factory.ResetDatabase;
    }

    [Fact]
    public async Task GetAllWorkBooks_WorkBooksExist_ReturnsPagedList()
    {
        await CreateWorkBookAsync();

        var response =
            await _workBooksApi.GetAllWorkBooksAsync(new GetAllRequest(1, 5));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var workBookResponse = response.Content!;
        workBookResponse.Should().NotBeNull();
        workBookResponse.Items.Should().NotBeEmpty();
    }

    // db contain preconfigured workBook
    [Fact]
    public async Task GetWorkBookById_WorkBookExist_ReturnsWorkBook()
    {
        var workBook = await CreateWorkBookAsync();

        var response =
            await _workBooksApi.GetWorkBookByIdAsync(workBook.Id);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var workBookResponse = response.Content;
        workBookResponse.Should().NotBeNull();
        workBookResponse!.Id.Should().Be(workBook.Id);
    }

    [Fact]
    public async Task GetWorkBookById_WorkBookNotExists_ReturnsNotFound()
    {
        var response = await _workBooksApi.GetWorkBookByIdAsync(Guid.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorMessage = await response.Error!.GetContentAsAsync<ErrorMessage>();
        errorMessage.Should().NotBeNull();
        errorMessage!.Code.Should().Be("WorkBook.NotFound");
    }

    [Fact]
    public async Task CreateWorkBook_WorkBookNotExists_ReturnsCreatedWithWorkBook()
    {
        var user = DataFakers.UserFaker.Generate();
        await UserManager.CreateAsync(user, "Pass123$");
        var workBook = DataFakers.WorkBookRequestFaker
            .RuleFor(wb => wb.UserId, _ => user.Id)
            .Generate();

        var response = await _workBooksApi.CreateWorkBookAsync(workBook);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var workBookResponse = response.Content;
        workBookResponse.Should().NotBeNull();
        workBookResponse!.Number.Should().Be(workBook.Number);
    }

    [Fact]
    public async Task UpdateWorkBook_WorkBooksExist_ReturnsOkWithUpdatedData()
    {
        var workBook = await CreateWorkBookAsync();

        var workBookRequest = DataFakers.WorkBookRequestFaker
            .RuleFor(wb => wb.UserId, _ => workBook.UserId)
            .Generate();

        var response = await _workBooksApi.UpdateWorkBookAsync(workBook.Id, workBookRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var workBookResponse = response.Content;
        workBookResponse.Should().NotBeNull();
        workBookResponse!.Number.Should().Be(workBookRequest.Number);
    }

    [Fact]
    public async Task UpdateWorkBook_WorkBookNotExist_ReturnsNotFound()
    {
        var workBook = DataFakers.WorkBookRequestFaker
            .RuleFor(wb => wb.UserId, _ => Guid.NewGuid())
            .Generate();

        var response = await _workBooksApi.UpdateWorkBookAsync(Guid.Empty, workBook);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorMessage = await response.Error!.GetContentAsAsync<ErrorMessage>();
        errorMessage.Should().NotBeNull();
        errorMessage!.Code.Should().Be("WorkBook.NotFound");
    }

    [Fact]
    public async Task DeleteWorkBook_WorkBooksExist_ReturnsNoContent()
    {
        var workBook = await CreateWorkBookAsync();

        var response = await _workBooksApi.DeleteWorkBooksAsync(workBook.Id);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteWorkBook_WorkBookNotExist_ReturnsNotFound()
    {
        var response = await _workBooksApi.DeleteWorkBooksAsync(Guid.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorMessage = await response.Error!.GetContentAsAsync<ErrorMessage>();
        errorMessage.Should().NotBeNull();
        errorMessage!.Code.Should().Be("WorkBook.NotFound");
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    private async Task<WorkBook> CreateWorkBookAsync()
    {
        var user = DataFakers.UserFaker.Generate();
        await UserManager.CreateAsync(user, "Pass123$");
        var workBook = DataFakers.WorkBookFaker
            .RuleFor(wb => wb.UserId, _ => user.Id)
            .Generate();
        await AddEntitiesToDbAsync(workBook);

        return workBook;
    }
}
