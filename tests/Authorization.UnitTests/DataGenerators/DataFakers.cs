using AuthorizationService.BLL.DTOs.Request;
using AuthorizationService.DAL.Constants;
using AuthorizationService.DAL.Models;
using Bogus;
using Microsoft.AspNetCore.Identity;

namespace Authorization.UnitTests.DataGenerators;

public static class DataFakers
{
    public static Faker<User> UserFaker => new Faker<User>()
        .RuleFor(u => u.Id, _ => Guid.NewGuid())
        .RuleFor(u => u.Email, f => f.Internet.ExampleEmail())
        .RuleFor(u => u.UserName, (_, u) => u.Email)
        .RuleFor(u => u.WorkBook, _ => null)
        .RuleFor(u => u.Passport, _ => null)
        .RuleFor(u => u.Contracts, _ => new List<Contract>())
        .RuleFor(u => u.Roles, _ => new List<IdentityRole<Guid>> { new (Roles.Administrator) });

    public static Faker<UserRequest> UserRequestFaker => new Faker<UserRequest>()
        .CustomInstantiator(
            f => new UserRequest(
                f.Internet.ExampleEmail(),
                f.Internet.Password(),
                Roles.Administrator));

    public static Faker<Contract> ContractFaker => new Faker<Contract>()
        .RuleFor(c => c.Id, _ => Guid.NewGuid())
        .RuleFor(c => c.Number, f => f.Random.AlphaNumeric(10))
        .RuleFor(c => c.StartDate, f => DateOnly.FromDateTime(f.Date.Past()))
        .RuleFor(c => c.EndDate, f => DateOnly.FromDateTime(f.Date.Future()))
        .RuleFor(c => c.User, _ => UserFaker.Generate())
        .RuleFor(c => c.UserId, (_, u) => u.Id);

    public static Faker<ContractRequest> ContractRequestFaker => new Faker<ContractRequest>()
        .CustomInstantiator(
            f => new ContractRequest(
                f.Random.AlphaNumeric(10),
                DateOnly.FromDateTime(f.Date.Past()).ToString(),
                DateOnly.FromDateTime(f.Date.Future()).ToString(),
                Guid.NewGuid()));

    public static Faker<Passport> PassportFaker => new Faker<Passport>()
        .RuleFor(p => p.Id, _ => Guid.NewGuid())
        .RuleFor(p => p.Series, f => f.Random.String(2, 'A', 'Z'))
        .RuleFor(p => p.Number, f => f.Random.String(7, '0', '9'))
        .RuleFor(p => p.IdentificationNumber, f => f.Random.AlphaNumeric(14))
        .RuleFor(p => p.Firstname, f => f.Name.FirstName())
        .RuleFor(p => p.Lastname, f => f.Name.LastName())
        .RuleFor(p => p.Patronymic, _ => null)
        .RuleFor(p => p.BirthDate, f => DateOnly.FromDateTime(f.Date.Past(30, DateTime.Now.AddYears(-18))))
        .RuleFor(p => p.IssueDate, f => DateOnly.FromDateTime(f.Date.Past(5)))
        .RuleFor(p => p.ExpiryDate, (_, p) => p.IssueDate.AddYears(10))
        .RuleFor(c => c.User, _ => UserFaker.Generate())
        .RuleFor(c => c.UserId, (_, u) => u.Id);

    public static Faker<PassportRequest> PassportRequestFaker => new Faker<PassportRequest>()
        .CustomInstantiator(
            f => new PassportRequest(
                f.Random.String(2, 'A', 'Z'),
                f.Random.String(7, '0', '9'),
                f.Random.AlphaNumeric(14),
                f.Name.FirstName(),
                f.Name.LastName(),
                null,
                DateOnly.FromDateTime(f.Date.Past(30, DateTime.Now.AddYears(-18))).ToString(),
                DateOnly.FromDateTime(f.Date.Past(5)).ToString(),
                null!,
                Guid.Empty))
        .RuleFor(p => p.ExpiryDate, (_, p) => DateOnly.Parse(p.IssueDate).AddYears(10).ToString());

    public static Faker<WorkBook> WorkBookFaker => new Faker<WorkBook>()
        .RuleFor(w => w.Id, f => Guid.NewGuid())
        .RuleFor(w => w.Number, f => f.Random.String(8, '0', '9'))
        .RuleFor(w => w.IssueDate, f => DateOnly.FromDateTime(f.Date.Past(5)))
        .RuleFor(c => c.User, _ => UserFaker.Generate())
        .RuleFor(c => c.UserId, (_, u) => u.Id);

    public static Faker<WorkBookRequest> WorkBookRequestFaker => new Faker<WorkBookRequest>()
        .CustomInstantiator(
            f => new WorkBookRequest(
                f.Random.String(8, '0', '9'),
                DateOnly.FromDateTime(f.Date.Past(5)).ToString(),
                Guid.Empty));

    public static Faker<IdentityRole<Guid>> RoleFaker => new Faker<IdentityRole<Guid>>()
        .RuleFor(w => w.Id, f => Guid.NewGuid())
        .RuleFor(w => w.Name, f => f.Random.String());
}
