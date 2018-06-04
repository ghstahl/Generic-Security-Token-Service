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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using P7.Core.Reflection;
using P7.GraphQLCore.Types;
using P7.GraphQLCore.Validators;

namespace P7.GraphQLCore
{
    internal class GraphQLJsonDocumentWriterOptions : IGraphQLJsonDocumentWriterOptions
    {
        public Formatting Formatting { get; set; }
        public JsonSerializerSettings JsonSerializerSettings { get; set; }
    }

    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            // This is a global sweep to find all types that
            // implement IMutationFieldRecordRegistration and IQueryFieldRecordRegistration.
            // We then register every one of them.
            // Future would be to database this, but for now if it is referenced it is in.

            // TODO: Trying out registration in each autofac module, vs a sweep

            /*
            var myTypes = TypeHelper<IQueryFieldRecordRegistration>
                .FindTypesInAssemblies(TypeHelper<IQueryFieldRecordRegistration>.IsType);
            foreach (var type in myTypes)
            {
                builder.RegisterType(type).As<IQueryFieldRecordRegistration>();
            }
            */
            builder.RegisterType<QueryFieldRecordRegistrationStore>()
                .As<IQueryFieldRecordRegistrationStore>()
                .SingleInstance();
            /*
            myTypes = TypeHelper<IMutationFieldRecordRegistration>
               .FindTypesInAssemblies(TypeHelper<IMutationFieldRecordRegistration>.IsType);
            foreach (var type in myTypes)
            {
               builder.RegisterType(type).As<IMutationFieldRecordRegistration>();
            }
             */
            builder.RegisterType<MutationFieldRecordRegistrationStore>()
                .As<IMutationFieldRecordRegistrationStore>()
                .SingleInstance();

            builder.RegisterType<GraphQLDocumentBuilder>().As<IDocumentBuilder>();
            builder.RegisterType<DocumentValidator>().As<IDocumentValidator>();
            builder.RegisterType<ComplexityAnalyzer>().As<IComplexityAnalyzer>();
            builder.RegisterType<DocumentExecuter>().As<IDocumentExecuter>().SingleInstance();

            builder.RegisterInstance(new GraphQLJsonDocumentWriterOptions
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
            }).As<IGraphQLJsonDocumentWriterOptions>();

            builder.RegisterType<GraphQLDocumentWriter>().As<IDocumentWriter>().SingleInstance();
            builder.RegisterType<QueryCore>().AsSelf();
            builder.RegisterType<MutationCore>().AsSelf();
            builder.RegisterType<SchemaCore>().As<ISchema>().SingleInstance();

            builder.Register<Func<Type, GraphType>>(c =>
            {
                var context = c.Resolve<IComponentContext>();
                return t =>
                {
                    var res = context.Resolve(t);
                    return (GraphType) res;
                };
            });


            builder.RegisterType<RequiresAuthValidationRule>()
                .As<IPluginValidationRule>()
                .SingleInstance();

            builder.RegisterType<OptOutGraphQLAuthorizationCheck>()
                .As<IGraphQLAuthorizationCheck>()
                .SingleInstance();

            builder.RegisterType<OptOutGraphQLClaimsAuthorizationCheck>()
                .As<IGraphQLClaimsAuthorizationCheck>()
                .SingleInstance();
            builder.RegisterType<DynamicType>();
            builder.RegisterType<PlaceHolderMutation>().As<IMutationFieldRecordRegistration>();

        }
    }
}