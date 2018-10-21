using GraphQL;
using GraphQLCore.ExtensionGrants.GraphQL;
using GraphQLCore.ExtensionGrants.GraphQL.Query;
using Microsoft.Extensions.DependencyInjection;
using P7.GraphQLCore;

namespace GraphQLCore.ExtensionGrants.Extensions
{
    public static class AspNetCoreServiceExtensions
    {
        public static void AddGraphQLCoreExtensionGrantsTypes(this IServiceCollection services)
        {
            services.AddTransient<IdentityModelType>();
            services.AddTransient<ClaimHandleType>();
            services.AddTransient<IQueryFieldRecordRegistration, IdentityQuery>();


            services.AddTransient<ArbitraryIdentityInput>();
            services.AddTransient<ArbitraryIdentityResultType>();
            services.AddTransient<IQueryFieldRecordRegistration, ArbitraryIdentityQuery>();

            services.AddTransient<ArbitraryResourceOwnerInput>();
            services.AddTransient<ArbitraryResourceOwnerResultType>();
            services.AddTransient<IQueryFieldRecordRegistration, ArbitraryResourceOwnerQuery>();

            services.AddTransient<ArbitraryNoSubjectInput>();
            services.AddTransient<ArbitraryNoSubjectResultType>();
            services.AddTransient<IQueryFieldRecordRegistration, ArbitraryNoSubjectQuery>();
        }
    }
}