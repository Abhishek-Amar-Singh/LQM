
using LQM.Web.Api.Services.DataExtractors;

public static partial class StartupSettings
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddFoundationServices(services);

        return services;
    }

    private static void AddFoundationServices(IServiceCollection services)
    {
        services.AddScoped<IDataExtractorService, DataExtractorService>();
    }
}

