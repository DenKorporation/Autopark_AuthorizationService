using AuthorizationService.BLL.DTOs.Request;
using AuthorizationService.BLL.DTOs.Response;
using AuthorizationService.BLL.Errors;
using AuthorizationService.BLL.Errors.Base;
using AuthorizationService.BLL.Services.Interfaces;
using AuthorizationService.DAL.Extensions;
using AuthorizationService.DAL.Models;
using AuthorizationService.DAL.Repositories.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationService.BLL.Services.Implementations;

public class PassportService(
    IPassportRepository passportRepository,
    IUserRepository userRepository,
    IUserClaimRepository userClaimRepository,
    IMapper mapper)
    : IPassportService
{
    public async Task<Result<PagedList<PassportResponse>>> GetAllAsync(
        GetAllRequest request,
        CancellationToken cancellationToken = default)
    {
        var resultQuery = await passportRepository.GetAllAsync(cancellationToken);

        var destQuery = resultQuery.ProjectTo<PassportResponse>(mapper.ConfigurationProvider);

        return await PagedList<PassportResponse>.CreateAsync(
            destQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }

    public async Task<Result<PassportResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await VerifyPassportExistsAsync(id, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToResult();
        }

        return mapper.Map<PassportResponse>(result.Value);
    }

    public async Task<Result<PassportResponse>> CreateAsync(
        PassportRequest request,
        CancellationToken cancellationToken = default)
    {
        var createPassport = mapper.Map<Passport>(request);

        // Validating
        var identificationNumberUniquenessResult = await VerifyIdentificationNumberUniquenessAsync(
            createPassport.IdentificationNumber,
            cancellationToken);

        if (identificationNumberUniquenessResult.IsFailed)
        {
            return identificationNumberUniquenessResult;
        }

        var uniquenessOfSeriesAndNumberResult = await VerifyUniquenessOfSeriesAndNumberAsync(
            createPassport.Series,
            createPassport.Number,
            cancellationToken);

        if (uniquenessOfSeriesAndNumberResult.IsFailed)
        {
            return uniquenessOfSeriesAndNumberResult;
        }

        var userResult = await VerifyUserExistsAsync(request.UserId, cancellationToken);

        if (userResult.IsFailed)
        {
            return userResult.ToResult();
        }

        var resultWorkBookNotExists = await VerifyPassportForUserNotExistsAsync(request.UserId, cancellationToken);

        if (resultWorkBookNotExists.IsFailed)
        {
            return resultWorkBookNotExists;
        }

        // Creating passport
        try
        {
            await passportRepository.CreateAsync(createPassport, cancellationToken);
        }
        catch (Exception)
        {
            return new InternalServerError("Passport.Create", "Something went wrong when saving the data");
        }

        // Creating user claims
        var createUserClaimResult = await userClaimRepository.AddClaimsAsync(
            userResult.Value,
            createPassport.ToClaims(),
            cancellationToken);

        if (!createUserClaimResult.Succeeded)
        {
            return new InternalServerError("Passport.Create", "Something went wrong when saving user claims");
        }

        return mapper.Map<PassportResponse>(createPassport);
    }

    public async Task<Result<PassportResponse>> UpdateAsync(
        Guid id,
        PassportRequest request,
        CancellationToken cancellationToken = default)
    {
        var resultPassport = await VerifyPassportExistsAsync(id, cancellationToken);

        if (resultPassport.IsFailed)
        {
            return resultPassport.ToResult();
        }

        var dbPassport = resultPassport.Value;

        if (dbPassport.IdentificationNumber != request.IdentificationNumber)
        {
            var identificationNumberUniquenessResult = await VerifyIdentificationNumberUniquenessAsync(
                request.IdentificationNumber,
                cancellationToken);

            if (identificationNumberUniquenessResult.IsFailed)
            {
                return identificationNumberUniquenessResult;
            }
        }

        if (dbPassport.Series != request.Series ||
            dbPassport.Number != request.Number)
        {
            var uniquenessOfSeriesAndNumberResult = await VerifyUniquenessOfSeriesAndNumberAsync(
                request.Series,
                request.Number,
                cancellationToken);

            if (uniquenessOfSeriesAndNumberResult.IsFailed)
            {
                return uniquenessOfSeriesAndNumberResult;
            }
        }

        var userResult = await VerifyUserExistsAsync(request.UserId, cancellationToken);

        if (userResult.IsFailed)
        {
            return userResult.ToResult();
        }

        // if user changes, checks are required
        if (dbPassport.UserId != request.UserId)
        {
            var resultPassportNotExists = await VerifyPassportForUserNotExistsAsync(request.UserId, cancellationToken);

            if (resultPassportNotExists.IsFailed)
            {
                return resultPassportNotExists;
            }
        }

        mapper.Map(request, dbPassport);

        try
        {
            await passportRepository.UpdateAsync(dbPassport, cancellationToken);
        }
        catch (Exception)
        {
            return new InternalServerError("Passport.Update", "Something went wrong when saving the data");
        }

        // Updating user claims
        var updateUserClaimResult = await userClaimRepository.UpdateClaimsAsync(
            userResult.Value,
            dbPassport.ToClaims(),
            cancellationToken);

        if (!updateUserClaimResult.Succeeded)
        {
            return new InternalServerError("Passport.Update", "Something went wrong when saving user claims");
        }

        return mapper.Map<PassportResponse>(dbPassport);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var resultPassport = await VerifyPassportExistsAsync(id, cancellationToken);

        if (resultPassport.IsFailed)
        {
            return resultPassport.ToResult();
        }

        var dbPassport = resultPassport.Value;

        try
        {
            await passportRepository.DeleteAsync(dbPassport, cancellationToken);
        }
        catch (Exception)
        {
            return new InternalServerError("Passport.Delete", "Something went wrong when removing the data");
        }

        return Result.Ok();
    }

    private async Task<Result> VerifyIdentificationNumberUniquenessAsync(
        string identificationNumber,
        CancellationToken cancellationToken = default)
    {
        var foundPassport = await passportRepository.GetByIdentificationNumberAsync(
            identificationNumber,
            cancellationToken);

        if (foundPassport is not null)
        {
            return new ConflictError(
                "Passport.Duplication",
                $"Passport '{identificationNumber}' already exist");
        }

        return Result.Ok();
    }

    private async Task<Result> VerifyUniquenessOfSeriesAndNumberAsync(
        string series,
        string number,
        CancellationToken cancellationToken = default)
    {
        var foundPassport = await passportRepository.GetBySeriesAndNumberAsync(
            series,
            number,
            cancellationToken);

        if (foundPassport is not null)
        {
            return new ConflictError(
                "Passport.Duplication",
                $"Passport '{series}{number}' already exist");
        }

        return Result.Ok();
    }

    private async Task<Result<Passport>> VerifyPassportExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var passport = await passportRepository.GetByIdAsync(id, cancellationToken);

        if (passport is null)
        {
            return new PassportNotFoundError(message: $"Passport '{id}' not found");
        }

        return passport;
    }

    private async Task<Result> VerifyPassportForUserNotExistsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var isPassportExist =
            await (await passportRepository.GetAllAsync(cancellationToken)).AnyAsync(
                p => p.UserId == userId,
                cancellationToken);

        if (isPassportExist)
        {
            return new ConflictError("Passport.Duplication", $"Passport for User '{userId}' already exist");
        }

        return Result.Ok();
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
