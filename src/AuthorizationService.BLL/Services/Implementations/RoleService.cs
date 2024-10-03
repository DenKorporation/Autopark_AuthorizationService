using AuthorizationService.BLL.DTOs.Response;
using AuthorizationService.BLL.Services.Interfaces;
using AuthorizationService.DAL.Repositories.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationService.BLL.Services.Implementations;

public class RoleService(
    IRoleRepository roleRepository,
    IMapper mapper)
    : IRoleService
{
    public async Task<Result<IEnumerable<RoleResponse>>> GetAllRolesAsync(CancellationToken cancellationToken = default)
    {
        var rolesQuery = await roleRepository.GetAllRolesAsync(cancellationToken);

        var rolesDestinationQuery = rolesQuery.ProjectTo<RoleResponse>(mapper.ConfigurationProvider);

        return await rolesDestinationQuery.ToListAsync(cancellationToken);
    }
}
