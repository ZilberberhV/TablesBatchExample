using Microsoft.Extensions.Configuration;

namespace TablesBatchExample;

public static class ConfigurationProvider
{
    private static IConfiguration? _config;

    public static IConfiguration Configuration 
    { 
        get
        {
            if (_config == null)
            {
                _config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();
            }

            return _config;
        }
    }
}
