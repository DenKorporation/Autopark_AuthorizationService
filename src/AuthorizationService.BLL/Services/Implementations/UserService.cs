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

namespace AuthorizationService.BLL.Services.Implementations;

public class UserService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUserClaimRepository userClaimRepository,
    IMapper mapper)
    : IUserService
{
    public async Task<Result<PagedList<UserResponse>>> GetAllAsync(
        UserFilterRequest request,
        CancellationToken cancellationToken = default)
    {
        var resultQuery = await userRepository.GetAllAsync(cancellationToken);

        resultQuery = FilterUsers(
            resultQuery,
            request.Role,
            request.BirthdateFrom is not null ? DateOnly.Parse(request.BirthdateFrom) : null,
            request.BirthdateTo is not null ? DateOnly.Parse(request.BirthdateTo) : null);

        var destQuery = resultQuery.ProjectTo<UserResponse>(mapper.ConfigurationProvider);

        return await PagedList<UserResponse>.CreateAsync(
            destQuery,
            request.PageNumber,
            request.PageSize,
            cancellationToken);
    }

    public async Task<Result<UserResponse>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var resultUser = await VerifyUserExistsAsync(email, cancellationToken);

        if (resultUser.IsFailed)
        {
            return resultUser.ToResult();
        }

        return mapper.Map<UserResponse>(resultUser.Value);
    }

    public async Task<Result<UserResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var resultUser = await VerifyUserExistsAsync(id, cancellationToken);

        if (resultUser.IsFailed)
        {
            return resultUser.ToResult();
        }

        return mapper.Map<UserResponse>(resultUser.Value);
    }

    public async Task<Result<UserResponse>> CreateAsync(
        UserRequest request,
        CancellationToken cancellationToken = default)
    {
        // Validating
        var roleResult = await VerifyRoleExistsAsync(request.Role, cancellationToken);

        if (roleResult.IsFailed)
        {
            return roleResult;
        }

        var passwordNotNullResult = VerifyPasswordNotNull(request.Password);

        if (passwordNotNullResult.IsFailed)
        {
            return passwordNotNullResult;
        }

        var emailUniquenessResult = await VerifyEmailUniquenessAsync(request.Email, cancellationToken);

        if (emailUniquenessResult.IsFailed)
        {
            return emailUniquenessResult;
        }

        // Creating user
        var createUser = mapper.Map<User>(request);

        var createResult = await userRepository.CreateAsync(createUser, request.Password!, cancellationToken);

        if (!createResult.Succeeded)
        {
            return new InternalServerError("User.Create", "Something went wrong when saving the data");
        }

        // Assigning role
        var assignRoleAsyncResult = await userRepository.AssignRoleAsync(createUser, request.Role, cancellationToken);

        if (!assignRoleAsyncResult.Succeeded)
        {
            return new InternalServerError("User.Create", "Something went wrong when assigning the role");
        }

        // Creating user claims
        var createUserClaimResult = await userClaimRepository.AddClaimsAsync(
            createUser,
            createUser.ToClaims(),
            cancellationToken);

        if (!createUserClaimResult.Succeeded)
        {
            return new InternalServerError("User.Create", "Something went wrong when saving user claims");
        }

        return mapper.Map<UserResponse>(createUser);
    }

    public async Task<Result<UserResponse>> UpdateAsync(
        Guid id,
        UserRequest request,
        CancellationToken cancellationToken = default)
    {
        // Validating
        var resultUser = await VerifyUserExistsAsync(id, cancellationToken);

        if (resultUser.IsFailed)
        {
            return resultUser.ToResult();
        }

        var dbUser = resultUser.Value;

        if (dbUser.Email != request.Email)
        {
            var emailUniquenessResult = await VerifyEmailUniquenessAsync(request.Email, cancellationToken);

            if (emailUniquenessResult.IsFailed)
            {
                return emailUniquenessResult;
            }
        }

        // Updating user
        mapper.Map(request, dbUser);

        var updateResult = await userRepository.UpdateUserAsync(dbUser, cancellationToken);

        if (!updateResult.Succeeded)
        {
            return new InternalServerError("User.Update", "Something went wrong when saving the data");
        }

        // Changing password
        if (request.Password is not null)
        {
            var changePasswordResult =
                await userRepository.ChangePasswordAsync(dbUser, request.Password, cancellationToken);

            if (!changePasswordResult.Succeeded)
            {
                return new InternalServerError("User.Update", "Something went wrong when changing the password");
            }
        }

        // Assigning role
        var dbUserRole = await userRepository.GetRoleAsync(dbUser, cancellationToken);

        if (dbUserRole != request.Role)
        {
            var roleResult = await VerifyRoleExistsAsync(request.Role, cancellationToken);

            if (roleResult.IsFailed)
            {
                return roleResult;
            }

            var changeRoleResult = await userRepository.AssignRoleAsync(dbUser, request.Role, cancellationToken);

            if (!changeRoleResult.Succeeded)
            {
                return new InternalServerError("User.Update", "Something went wrong when assigning the role");
            }
        }

        // Updating user claims
        var updateUserClaimResult = await userClaimRepository.UpdateClaimsAsync(
            dbUser,
            dbUser.ToClaims(),
            cancellationToken);

        if (!updateUserClaimResult.Succeeded)
        {
            return new InternalServerError("User.Update", "Something went wrong when saving user claims");
        }

        return mapper.Map<UserResponse>(dbUser);
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var resultUser = await VerifyUserExistsAsync(id, cancellationToken);

        if (resultUser.IsFailed)
        {
            return resultUser.ToResult();
        }

        var dbUser = resultUser.Value;

        try
        {
            await userRepository.DeleteUserAsync(dbUser, cancellationToken);
        }
        catch (Exception)
        {
            return new InternalServerError("User.Delete", "Something went wrong when removing the data");
        }

        return Result.Ok();
    }

    private IQueryable<User> FilterUsers(
        IQueryable<User> queryable,
        string? role,
        DateOnly? birthdateFrom,
        DateOnly? birthdateTo)
    {
        if (role is not null)
        {
            queryable = queryable.Where(
                c => c.Roles.Any(r => r.Name!.Equals(role, StringComparison.InvariantCultureIgnoreCase)));
        }

        if (birthdateFrom is not null)
        {
            queryable = queryable.Where(c => c.Passport != null && c.Passport.BirthDate >= birthdateFrom);
        }

        if (birthdateTo is not null)
        {
            queryable = queryable.Where(c => c.Passport != null && c.Passport.BirthDate <= birthdateTo);
        }

        return queryable;
    }

    private async Task<Result> VerifyEmailUniquenessAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var foundUser = await userRepository.GetByEmailAsync(
            email,
            cancellationToken);

        if (foundUser is not null)
        {
            return new ConflictError(
                "User.Duplication",
                $"User '{email}' already exist");
        }

        return Result.Ok();
    }

    private Result VerifyPasswordNotNull(string? password)
    {
        if (password is null)
        {
            return new ValidationError(
                new Dictionary<string, string[]> { { "Password", ["Password was expected"] } },
                "User.Validation");
        }

        return Result.Ok();
    }

    private async Task<Result> VerifyRoleExistsAsync(string roleName, CancellationToken cancellationToken = default)
    {
        if (!await roleRepository.RoleExistsAsync(roleName, cancellationToken))
        {
            return new RoleNotFoundError(message: $"Role '{roleName}' not found");
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

    private async Task<Result<User>> VerifyUserExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            return new UserNotFoundError(message: $"User '{email}' not found");
        }

        return user;
    }
}
