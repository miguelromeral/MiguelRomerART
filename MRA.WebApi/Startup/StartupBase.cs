using MRA.DTO.Options;

namespace MRA.WebApi.Startup;

public abstract class StartupBase<T> where T : class
{
    public static T GetConfigSection(string sectionName, IServiceCollection services, IConfiguration configuration)
    {   
        var section = configuration.GetSection(sectionName);
        services.Configure<T>(section);
        return section.Get<T>();
    }
}
