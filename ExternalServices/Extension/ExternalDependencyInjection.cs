using ExternalServices.Facades;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Net.Http;

namespace ExternalServices.Extension
{
    public static class ExternalDependencyInjection
    {
        public static IServiceCollection AddExternalServicesConfiguration(this IServiceCollection services,
        IConfiguration configuration)
        {
            var settings = new RefitSettings();
            settings.HttpMessageHandlerFactory = () => new HttpClientHandler()
            {
                UseCookies = false
            };

            services.AddRefitClient<IHomeFamilyPointsFacade>()
                    .ConfigureHttpClient((sp, client) =>
                    {
                        var baseUrl = configuration["ExternalUrl"];
                        client.BaseAddress = new Uri(baseUrl);
                    });

            return services;
        }
    }
}
