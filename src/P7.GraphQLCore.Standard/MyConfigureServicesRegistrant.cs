using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using P7.Core.Settings;
using P7.Core.Startup;
using P7.GraphQLCore.Stores;


namespace P7.GraphQLCore
{

    public class MutationConfig
    {
        public List<string> OptOut { get; set; }
    }
    public class QueryConfig
    {
        public List<string> OptOut { get; set; }
    }
    public class GraphQLAuthenticationConfig
    {
        public const string WellKnown_SectionName = "graphQLAuthentication";
        public MutationConfig Mutation { get; set; }
        public QueryConfig Query { get; set; }
    }
    public static class ConfigurationServicesExtension
    {
        public static void RegisterGraphQLCoreConfigurationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<GraphQLAuthenticationConfig>(configuration.GetSection(GraphQLAuthenticationConfig.WellKnown_SectionName));
            services.Configure<GraphQLFieldAuthorityConfig>(configuration.GetSection(GraphQLFieldAuthorityConfig.WellKnown_SectionName));
        }
    }
}