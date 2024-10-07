using System.Net;
using Authorization.IntegrationTests.DataGenerators;
using Authorization.IntegrationTests.Responses;
using Authorization.IntegrationTests.RestApis.Interfaces;
using AuthorizationService.BLL.DTOs.Request;
using AuthorizationService.DAL.Models;
using FluentAssertions;
using Refit;

namespace Authorization.IntegrationTests.Controllers;

public class UserControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly IUsersApi _usersApi;
    private readonly Func<Task> _resetDatabase;

    public UserControllerTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
        _usersApi = RestService.For<IUsersApi>(Client);
        _resetDatabase = factory.ResetDatabase;
    }

    [Fact]
    public async Task GetAllUsers_UsersExist_ReturnsPagedList()
    {
        await CreateUserAsync();

        var response =
            await _usersApi.GetAllUsersAsync(
                new UserFilterRequest(
                    1,
                    5,
                    null,
                    null,
                    null));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var userResponse = response.Content!;
        userResponse.Should().NotBeNull();
        userResponse.Items.Should().NotBeEmpty();
    }

    // db contain preconfigured user
    [Fact]
    public async Task GetUserById_UserExist_ReturnsUser()
    {
        var user = await CreateUserAsync();

        var response =
            await _usersApi.GetUserByIdAsync(user.Id);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var userResponse = response.Content;
        userResponse.Should().NotBeNull();
        userResponse!.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetUserById_UserNotExists_ReturnsNotFound()
    {
        var response = await _usersApi.GetUserByIdAsync(Guid.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorMessage = await response.Error!.GetContentAsAsync<ErrorMessage>();
        errorMessage.Should().NotBeNull();
        errorMessage!.Code.Should().Be("User.NotFound");
    }

    [Fact]
    public async Task CreateUser_UserNotExists_ReturnsCreatedWithUser()
    {
        var user = DataFakers.UserRequestFaker.Generate();

        var response = await _usersApi.CreateUserAsync(user);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var userResponse = response.Content;
        userResponse.Should().NotBeNull();
        userResponse!.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task UpdateUser_UsersExist_ReturnsOkWithUpdatedData()
    {
        var user = await CreateUserAsync();

        var userRequest = DataFakers.UserRequestFaker.Generate();

        var response = await _usersApi.UpdateUserAsync(user.Id, userRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var userResponse = response.Content;
        userResponse.Should().NotBeNull();
        userResponse!.Email.Should().Be(userRequest.Email);
    }

    [Fact]
    public async Task UpdateUser_UserNotExist_ReturnsNotFound()
    {
        var user = DataFakers.UserRequestFaker.Generate();

        var response = await _usersApi.UpdateUserAsync(Guid.Empty, user);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorMessage = await response.Error!.GetContentAsAsync<ErrorMessage>();
        errorMessage.Should().NotBeNull();
        errorMessage!.Code.Should().Be("User.NotFound");
    }

    [Fact]
    public async Task DeleteUser_UsersExist_ReturnsNoContent()
    {
        var user = await CreateUserAsync();

        var response = await _usersApi.DeleteUsersAsync(user.Id);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteUser_UserNotExist_ReturnsNotFound()
    {
        var response = await _usersApi.DeleteUsersAsync(Guid.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorMessage = await response.Error!.GetContentAsAsync<ErrorMessage>();
        errorMessage.Should().NotBeNull();
        errorMessage!.Code.Should().Be("User.NotFound");
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync() => await _resetDatabase();

    private async Task<User> CreateUserAsync()
    {
        var user = DataFakers.UserFaker.Generate();
        await UserManager.CreateAsync(user, "Pass123$");

        return user;
    }
}
