using System.Net;
using Authorization.IntegrationTests.DataGenerators;
using Authorization.IntegrationTests.Responses;
using Authorization.IntegrationTests.RestApis.Interfaces;
using AuthorizationService.BLL.DTOs.Request;
using AuthorizationService.DAL.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Refit;

namespace Authorization.IntegrationTests.Controllers;

public class PassportControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly IPassportsApi _passportsApi;
    private readonly Func<Task> _resetDatabase;

    public PassportControllerTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
        _passportsApi = RestService.For<IPassportsApi>(Client);
        _resetDatabase = factory.ResetDatabase;
    }

    [Fact]
    public async Task GetAllPassports_PassportsExist_ReturnsPagedList()
    {
        await CreatePassportAsync();

        var response =
            await _passportsApi.GetAllPassportsAsync(new GetAllRequest(1, 5));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var passportResponse = response.Content!;
        passportResponse.Should().NotBeNull();
        passportResponse.Items.Should().NotBeEmpty();
    }

    // db contain preconfigured passport
    [Fact]
    public async Task GetPassportById_PassportExist_ReturnsPassport()
    {
        var passport = await CreatePassportAsync();

        var response =
            await _passportsApi.GetPassportByIdAsync(passport.Id);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var passportResponse = response.Content;
        passportResponse.Should().NotBeNull();
        passportResponse!.Id.Should().Be(passport.Id);
    }

    [Fact]
    public async Task GetPassportById_PassportNotExists_ReturnsNotFound()
    {
        var response = await _passportsApi.GetPassportByIdAsync(Guid.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorMessage = await response.Error!.GetContentAsAsync<ErrorMessage>();
        errorMessage.Should().NotBeNull();
        errorMessage!.Code.Should().Be("Passport.NotFound");
    }

    [Fact]
    public async Task CreatePassport_PassportNotExists_ReturnsCreatedWithPassport()
    {
        var user = DataFakers.UserFaker.Generate();
        await UserManager.CreateAsync(user, "Pass123$");
        var passport = DataFakers.PassportRequestFaker
            .RuleFor(p => p.UserId, _ => user.Id)
            .Generate();

        var response = await _passportsApi.CreatePassportAsync(passport);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var passportResponse = response.Content;
        passportResponse.Should().NotBeNull();
        passportResponse!.Number.Should().Be(passport.Number);
    }

    [Fact]
    public async Task UpdatePassport_PassportsExist_ReturnsOkWithUpdatedData()
    {
        var passport = await CreatePassportAsync();

        var passportRequest = DataFakers.PassportRequestFaker
            .RuleFor(p => p.UserId, _ => passport.UserId)
            .Generate();

        var response = await _passportsApi.UpdatePassportAsync(passport.Id, passportRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var passportResponse = response.Content;
        passportResponse.Should().NotBeNull();
        passportResponse!.Number.Should().Be(passportRequest.Number);
    }

    [Fact]
    public async Task UpdatePassport_PassportNotExist_ReturnsNotFound()
    {
        var passport = DataFakers.PassportRequestFaker
            .RuleFor(p => p.UserId, _ => Guid.NewGuid())
            .Generate();

        var response = await _passportsApi.UpdatePassportAsync(Guid.Empty, passport);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorMessage = await response.Error!.GetContentAsAsync<ErrorMessage>();
        errorMessage.Should().NotBeNull();
        errorMessage!.Code.Should().Be("Passport.NotFound");
    }

    [Fact]
    public async Task DeletePassport_PassportsExist_ReturnsNoContent()
    {
        var passport = await CreatePassportAsync();

        var response = await _passportsApi.DeletePassportsAsync(passport.Id);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeletePassport_PassportNotExist_ReturnsNotFound()
    {
        var response = await _passportsApi.DeletePassportsAsync(Guid.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorMessage = await response.Error!.GetContentAsAsync<ErrorMessage>();
        errorMessage.Should().NotBeNull();
        errorMessage!.Code.Should().Be("Passport.NotFound");
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    private async Task<Passport> CreatePassportAsync()
    {
        var user = DataFakers.UserFaker.Generate();
        await UserManager.CreateAsync(user, "Pass123$");
        var passport = DataFakers.PassportFaker
            .RuleFor(p => p.UserId, _ => user.Id)
            .Generate();
        await AddEntitiesToDbAsync(passport);

        return passport;
    }
}
