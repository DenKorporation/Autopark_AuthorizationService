using AuthorizationService.DAL.Contexts;
using AuthorizationService.DAL.Models;
using AuthorizationService.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationService.DAL.Repositories.Implementations;

public class PassportRepository(AuthContext dbContext)
    : Repository<Passport>(dbContext), IPassportRepository
{
    private readonly AuthContext _dbContext = dbContext;

    public async Task<Passport?> GetByIdentificationNumberAsync(
        string identificationNumber,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Passports
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdentificationNumber == identificationNumber, cancellationToken);
    }

    public async Task<Passport?> GetBySeriesAndNumberAsync(
        string series,
        string number,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext
            .Passports
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Series == series && p.Number == number, cancellationToken);
    }
}
