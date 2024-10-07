using Asp.Versioning.ApiExplorer;
using AuthorizationService.API.Extensions;
using AuthorizationService.BLL.Extensions;
using AuthorizationService.DAL.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDalServices(builder.Configuration);
builder.Services.AddBllServices(builder.Configuration);
builder.Services.AddPresentationServices(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(
        options =>
        {
            IReadOnlyList<ApiVersionDescription> descriptions = app.DescribeApiVersions();

            foreach (var description in descriptions)
            {
                string url = $"/swagger/{description.GroupName}/swagger.json";
                string name = description.GroupName.ToUpperInvariant();

                options.SwaggerEndpoint(url, name);
            }

            options.OAuthClientId("dev");
        });

    await app.InitializeDatabaseAsync();
    await app.InitializeOperationStoreAsync();
}

app.UseIdentityServer();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}
