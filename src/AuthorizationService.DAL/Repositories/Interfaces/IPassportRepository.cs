using AuthorizationService.DAL.Models;

namespace AuthorizationService.DAL.Repositories.Interfaces;

public interface IPassportRepository : IRepository<Passport>
{
    public Task<Passport?> GetByIdentificationNumberAsync(
        string identificationNumber,
        CancellationToken cancellationToken = default);

    public Task<Passport?> GetBySeriesAndNumberAsync(
        string series,
        string number,
        CancellationToken cancellationToken = default);
}
