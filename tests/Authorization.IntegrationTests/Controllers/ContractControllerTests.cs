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

public class ContractControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private readonly IContractsApi _contractsApi;
    private readonly Func<Task> _resetDatabase;

    public ContractControllerTests(CustomWebApplicationFactory factory)
        : base(factory)
    {
        _contractsApi = RestService.For<IContractsApi>(Client);
        _resetDatabase = factory.ResetDatabase;
    }

    [Fact]
    public async Task GetAllContracts_ContractsExist_ReturnsPagedList()
    {
        await CreateContractAsync();

        var response =
            await _contractsApi.GetAllContractsAsync(
                new ContractFilterRequest(
                    1,
                    5,
                    null,
                    null,
                    null,
                    null,
                    null));

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var contractResponse = response.Content!;
        contractResponse.Should().NotBeNull();
        contractResponse.Items.Should().NotBeEmpty();
    }

    // db contain preconfigured contract
    [Fact]
    public async Task GetContractById_ContractExist_ReturnsContract()
    {
        var contract = await TestDbContext.Contracts.FirstOrDefaultAsync();

        var response =
            await _contractsApi.GetContractByIdAsync(contract!.Id);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var contractResponse = response.Content;
        contractResponse.Should().NotBeNull();
        contractResponse!.Id.Should().Be(contract.Id);
    }

    [Fact]
    public async Task GetContractById_ContractNotExists_ReturnsNotFound()
    {
        var response = await _contractsApi.GetContractByIdAsync(Guid.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorMessage = await response.Error!.GetContentAsAsync<ErrorMessage>();
        errorMessage.Should().NotBeNull();
        errorMessage!.Code.Should().Be("Contract.NotFound");
    }

    [Fact]
    public async Task CreateContract_ContractNotExists_ReturnsCreatedWithContract()
    {
        var user = DataFakers.UserFaker.Generate();
        await UserManager.CreateAsync(user, "Pass123$");
        var contract = DataFakers.ContractRequestFaker
            .RuleFor(c => c.UserId, _ => user.Id)
            .Generate();

        var response = await _contractsApi.CreateContractAsync(contract);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var contractResponse = response.Content;
        contractResponse.Should().NotBeNull();
        contractResponse!.Number.Should().Be(contract.Number);
    }

    [Fact]
    public async Task UpdateContract_ContractsExist_ReturnsOkWithUpdatedData()
    {
        var contract = await CreateContractAsync();

        var contractRequest = DataFakers.ContractRequestFaker
            .RuleFor(c => c.UserId, _ => contract.UserId)
            .Generate();

        var response = await _contractsApi.UpdateContractAsync(contract.Id, contractRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var contractResponse = response.Content;
        contractResponse.Should().NotBeNull();
        contractResponse!.Number.Should().Be(contractRequest.Number);
    }

    [Fact]
    public async Task UpdateContract_ContractNotExist_ReturnsNotFound()
    {
        var contract = DataFakers.ContractRequestFaker.Generate();

        var response = await _contractsApi.UpdateContractAsync(Guid.Empty, contract);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorMessage = await response.Error!.GetContentAsAsync<ErrorMessage>();
        errorMessage.Should().NotBeNull();
        errorMessage!.Code.Should().Be("Contract.NotFound");
    }

    [Fact]
    public async Task DeleteContract_ContractsExist_ReturnsNoContent()
    {
        var contract = await CreateContractAsync();

        var response = await _contractsApi.DeleteContractsAsync(contract.Id);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DeleteContract_ContractNotExist_ReturnsNotFound()
    {
        var response = await _contractsApi.DeleteContractsAsync(Guid.Empty);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var errorMessage = await response.Error!.GetContentAsAsync<ErrorMessage>();
        errorMessage.Should().NotBeNull();
        errorMessage!.Code.Should().Be("Contract.NotFound");
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _resetDatabase();

    private async Task<Contract> CreateContractAsync()
    {
        var user = DataFakers.UserFaker.Generate();
        await UserManager.CreateAsync(user, "Pass123$");
        var contract = DataFakers.ContractFaker
            .RuleFor(c => c.UserId, _ => user.Id)
            .Generate();
        await AddEntitiesToDbAsync(contract);

        return contract;
    }
}
