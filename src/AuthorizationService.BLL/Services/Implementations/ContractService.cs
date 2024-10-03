using AuthorizationService.BLL.DTOs.Request;
using AuthorizationService.BLL.DTOs.Response;
using AuthorizationService.BLL.Errors;
using AuthorizationService.BLL.Errors.Base;
using AuthorizationService.BLL.Services.Interfaces;
using AuthorizationService.DAL.Models;
using AuthorizationService.DAL.Repositories.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentResults;

namespace AuthorizationService.BLL.Services.Implementations;

public class ContractService(
    IContractRepository contractRepository,
    IUserRepository userRepository,
    IMapper mapper)
    : IContractService
{
    public async Task<Result<PagedList<ContractResponse>>> GetAllAsync(
        ContractFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var resultQuery = await contractRepository.GetAllAsync(cancellationToken);

        resultQuery = FilterContracts(
            resultQuery,
            request.Number,
            request.StartDate is not null ? DateOnly.Parse(request.StartDate) : null,
            request.EndDate is not null ? DateOnly.Parse(request.EndDate) : null,
            request.IsValid,
            request.UserId);

        var destQuery = resultQuery.ProjectTo<ContractResponse>(mapper.ConfigurationProvider);

        return await PagedList<ContractResponse>.CreateAsync(
            destQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }

    public async Task<Result<ContractResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await VerifyContractExistsAsync(id, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToResult();
        }

        return mapper.Map<ContractResponse>(result.Value);
    }

    public async Task<Result<ContractResponse>> CreateAsync(
        ContractRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = await VerifyUserExistsAsync(request.UserId, cancellationToken);

        if (userResult.IsFailed)
        {
            return userResult.ToResult();
        }

        var createContract = mapper.Map<Contract>(request);

        try
        {
            await contractRepository.CreateAsync(createContract, cancellationToken);
        }
        catch (Exception)
        {
            return new InternalServerError("Contract.Create", "Something went wrong when saving the data");
        }

        return mapper.Map<ContractResponse>(createContract);
    }

    public async Task<Result<ContractResponse>> UpdateAsync(
        Guid id,
        ContractRequest request,
        CancellationToken cancellationToken = default)
    {
        var resultContract = await VerifyContractExistsAsync(id, cancellationToken);

        if (resultContract.IsFailed)
        {
            return resultContract.ToResult();
        }

        var dbContract = resultContract.Value;

        // if user changes, checks are required
        if (dbContract.UserId != request.UserId)
        {
            var userResult = await VerifyUserExistsAsync(request.UserId, cancellationToken);

            if (userResult.IsFailed)
            {
                return userResult.ToResult();
            }
        }

        mapper.Map(request, dbContract);

        try
        {
            await contractRepository.UpdateAsync(dbContract, cancellationToken);
        }
        catch (Exception)
        {
            return new InternalServerError("Contract.Update", "Something went wrong when saving the data");
        }

        return mapper.Map<ContractResponse>(dbContract);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var resultContract = await VerifyContractExistsAsync(id, cancellationToken);

        if (resultContract.IsFailed)
        {
            return resultContract.ToResult();
        }

        var dbContract = resultContract.Value;

        try
        {
            await contractRepository.DeleteAsync(dbContract, cancellationToken);
        }
        catch (Exception)
        {
            return new InternalServerError("Contract.Delete", "Something went wrong when removing the data");
        }

        return Result.Ok();
    }

    private IQueryable<Contract> FilterContracts(
        IQueryable<Contract> queryable,
        string? number,
        DateOnly? startDate,
        DateOnly? endDate,
        bool? isValid,
        Guid? userId)
    {
        if (number is not null)
        {
            queryable = queryable.Where(c => c.Number.Contains(number, StringComparison.InvariantCultureIgnoreCase));
        }

        if (startDate is not null)
        {
            queryable = queryable.Where(c => c.StartDate >= startDate);
        }

        if (endDate is not null)
        {
            queryable = queryable.Where(c => c.EndDate <= endDate);
        }

        if (isValid is not null)
        {
            queryable = queryable.Where(c => DateOnly.FromDateTime(DateTime.Today) <= c.EndDate);
        }

        if (userId is not null)
        {
            queryable = queryable.Where(c => c.UserId == userId);
        }

        return queryable;
    }

    private async Task<Result<Contract>> VerifyContractExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var contract = await contractRepository.GetByIdAsync(id, cancellationToken);

        if (contract is null)
        {
            return new ContractNotFoundError(message: $"Contract '{id}' not found");
        }

        return contract;
    }

    private async Task<Result<User>> VerifyUserExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return new UserNotFoundError(message: $"User '{id}' not found");
        }

        return user;
    }
}
