using System;
using System.Net.Http;
using Elivn.AliyunOssSdk;
using Elivn.AliyunOssSdk.Api.Bucket.List;
using Elivn.AliyunOssSdk.Entites;
using Elivn.AliyunOssSdk.Request;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OssClientServiceCollectionExtensions
    {
        public static IServiceCollection AddOssClient(
            this IServiceCollection services,
            IConfiguration configuration, 
            string sectionName = "ossClient",
            Action<ClientConfiguration> setupClientConfiguration = null,
            Action<HttpClient> configureHttpClient = null)
        {
            var section = (configuration as IConfigurationSection) == null ?
                configuration.GetSection("ossClient") : configuration;

            services.Configure<OssCredential>(section);

            var clientConfiguration = new ClientConfiguration();
            setupClientConfiguration?.Invoke(clientConfiguration);
            services.AddSingleton(clientConfiguration);

            services.AddTransient<RequestContext>();

            if (configureHttpClient == null)
            {
                services.AddHttpClient<OssClient>();
            }
            else
            {
                services.AddHttpClient<OssClient>(configureHttpClient);
            }

            return services;
        }
    }
}