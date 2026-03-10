using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dealmatcher.Backend.Infrastructure.Configs;

public static class AutoMapperConfig
{
    public static IServiceCollection AddAutoMapperConfigs(this IServiceCollection services)
    {
        services.AddAutoMapper(
            _ =>
            {

            });

        return services;
    }
}
