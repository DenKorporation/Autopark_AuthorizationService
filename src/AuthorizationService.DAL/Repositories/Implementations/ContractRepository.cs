using AuthorizationService.DAL.Contexts;
using AuthorizationService.DAL.Models;
using AuthorizationService.DAL.Repositories.Interfaces;

namespace AuthorizationService.DAL.Repositories.Implementations;

public class ContractRepository(AuthContext dbContext)
    : Repository<Contract>(dbContext), IContractRepository
{
}
