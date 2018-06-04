using System;
using System.Collections.Generic;
using AuthHandler.GraphQL;
using Autofac;
using GraphQL;
using GraphQL.Execution;
using GraphQL.Http;
using GraphQL.Types;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using P7.GraphQLCore.Types;
using P7.GraphQLCore.Validators;

namespace P7.GraphQLCore.Extensions
{
    public static class AspNetCoreServiceExtensions
    {
        public static void AddGraphQLCoreTypes(this IServiceCollection services)
        {
            services.AddSingleton< IQueryFieldRecordRegistrationStore,QueryFieldRecordRegistrationStore>();
            services.AddSingleton<IMutationFieldRecordRegistrationStore, MutationFieldRecordRegistrationStore>();

            services.AddTransient<IDocumentBuilder, GraphQLDocumentBuilder>();
            services.AddTransient<IDocumentValidator, DocumentValidator>();
            services.AddTransient<IComplexityAnalyzer, ComplexityAnalyzer>();
            services.AddSingleton<IDocumentExecuter, DocumentExecuter>();

            services.TryAddTransient<IGraphQLJsonDocumentWriterOptions>(
                _ =>
                {
                    var graphQLJsonDocumentWriterOptions = new GraphQLJsonDocumentWriterOptions
                    {
                        Formatting = Formatting.None,
                        JsonSerializerSettings = new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver(),
                            DateFormatHandling = DateFormatHandling.IsoDateFormat,
                            DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'",
                            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                            Converters = new List<JsonConverter>()
                            {
                                new Newtonsoft.Json.Converters.IsoDateTimeConverter()
                            }
                        }
                    };
                    return graphQLJsonDocumentWriterOptions;
                });

            services.AddSingleton<IDocumentWriter, GraphQLDocumentWriter>();
            services.AddTransient<QueryCore>();
            services.AddTransient<MutationCore>();
            services.AddSingleton<ISchema, SchemaCore>();
            services.TryAddTransient<Func<Type, GraphType>>(
                x =>
                {
                    var context = x.GetService<IComponentContext>();
                    return t =>
                    {
                        var res = context.Resolve(t);
                        return (GraphType)res;
                    };
                });

            services.AddSingleton<IPluginValidationRule, RequiresAuthValidationRule>();
            services.AddSingleton<IGraphQLAuthorizationCheck, OptOutGraphQLAuthorizationCheck>();
            services.AddSingleton<IGraphQLClaimsAuthorizationCheck, OptOutGraphQLClaimsAuthorizationCheck>();

            services.AddTransient<DynamicType>();
            services.AddTransient<IMutationFieldRecordRegistration, PlaceHolderMutation>();
        }
    }
}