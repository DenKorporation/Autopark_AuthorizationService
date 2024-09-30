using AuthorizationService.DAL.Contexts;
using AuthorizationService.DAL.Models;
using AuthorizationService.DAL.Repositories.Interfaces;

namespace AuthorizationService.DAL.Repositories.Implementations;

public class WorkBookRepository(AuthContext dbContext)
    : Repository<WorkBook>(dbContext), IWorkBookRepository
{
}
