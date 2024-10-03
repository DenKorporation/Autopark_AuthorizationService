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
using Microsoft.EntityFrameworkCore;

namespace AuthorizationService.BLL.Services.Implementations;

public class WorkBookService(
    IWorkBookRepository workBookRepository,
    IUserRepository userRepository,
    IMapper mapper)
    : IWorkBookService
{
    public async Task<Result<PagedList<WorkBookResponse>>> GetAllAsync(
        GetAllRequest request,
        CancellationToken cancellationToken = default)
    {
        var resultQuery = await workBookRepository.GetAllAsync(cancellationToken);

        var destQuery = resultQuery.ProjectTo<WorkBookResponse>(mapper.ConfigurationProvider);

        return await PagedList<WorkBookResponse>.CreateAsync(
            destQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }

    public async Task<Result<WorkBookResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await VerifyWorkBookExistsAsync(id, cancellationToken);

        if (result.IsFailed)
        {
            return result.ToResult();
        }

        return mapper.Map<WorkBookResponse>(result.Value);
    }

    public async Task<Result<WorkBookResponse>> CreateAsync(
        WorkBookRequest request,
        CancellationToken cancellationToken = default)
    {
        var userResult = await VerifyUserExistsAsync(request.UserId, cancellationToken);

        if (userResult.IsFailed)
        {
            return userResult.ToResult();
        }

        var resultWorkBookNotExists = await VerifyWorkBookForUserNotExistsAsync(request.UserId, cancellationToken);

        if (resultWorkBookNotExists.IsFailed)
        {
            return resultWorkBookNotExists;
        }

        var createWorkBook = mapper.Map<WorkBook>(request);

        try
        {
            await workBookRepository.CreateAsync(createWorkBook, cancellationToken);
        }
        catch (Exception)
        {
            return new InternalServerError("WorkBook.Create", "Something went wrong when saving the data");
        }

        return mapper.Map<WorkBookResponse>(createWorkBook);
    }

    public async Task<Result<WorkBookResponse>> UpdateAsync(
        Guid id,
        WorkBookRequest request,
        CancellationToken cancellationToken = default)
    {
        var resultWorkBook = await VerifyWorkBookExistsAsync(id, cancellationToken);

        if (resultWorkBook.IsFailed)
        {
            return resultWorkBook.ToResult();
        }

        var dbWorkBook = resultWorkBook.Value;

        // if user changes, checks are required
        if (dbWorkBook.UserId != request.UserId)
        {
            var userResult = await VerifyUserExistsAsync(request.UserId, cancellationToken);

            if (userResult.IsFailed)
            {
                return userResult.ToResult();
            }

            var resultWorkBookNotExists = await VerifyWorkBookForUserNotExistsAsync(request.UserId, cancellationToken);

            if (resultWorkBookNotExists.IsFailed)
            {
                return resultWorkBookNotExists;
            }
        }

        mapper.Map(request, dbWorkBook);

        try
        {
            await workBookRepository.UpdateAsync(dbWorkBook, cancellationToken);
        }
        catch (Exception)
        {
            return new InternalServerError("WorkBook.Update", "Something went wrong when saving the data");
        }

        return mapper.Map<WorkBookResponse>(dbWorkBook);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var resultWorkBook = await VerifyWorkBookExistsAsync(id, cancellationToken);

        if (resultWorkBook.IsFailed)
        {
            return resultWorkBook.ToResult();
        }

        var dbWorkBook = resultWorkBook.Value;

        try
        {
            await workBookRepository.DeleteAsync(dbWorkBook, cancellationToken);
        }
        catch (Exception)
        {
            return new InternalServerError("WorkBook.Delete", "Something went wrong when removing the data");
        }

        return Result.Ok();
    }

    private async Task<Result<WorkBook>> VerifyWorkBookExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workBook = await workBookRepository.GetByIdAsync(id, cancellationToken);

        if (workBook is null)
        {
            return new WorkBookNotFoundError(message: $"WorkBook '{id}' not found");
        }

        return workBook;
    }

    private async Task<Result> VerifyWorkBookForUserNotExistsAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var isWorkBookExist =
            await (await workBookRepository.GetAllAsync(cancellationToken)).AnyAsync(
                wb => wb.UserId == userId,
                cancellationToken);

        if (isWorkBookExist)
        {
            return new ConflictError("WorkBook.Duplication", $"WorkBook for User '{userId}' already exist");
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
