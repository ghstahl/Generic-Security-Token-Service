using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Autofac;
using P7.Core.Localization;
using P7.Core.Localization.Treatment;
using P7.Core.Middleware;
using P7.Core.Reflection;
using Serilog;
using Module = Autofac.Module;

namespace P7.Core
{
    public class AutofacModule : Module
    {
        static ILogger logger = Log.ForContext<AutofacModule>();
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Global>().SingleInstance();

            logger.Information("Hi from P7.Core Autofac.Load!");
            var assembly = this.GetType().GetTypeInfo().Assembly;
            var derivedTypes = TypeHelper<MiddlewarePlugin>.FindDerivedTypes(assembly).ToArray();
            var derivedTypesName = derivedTypes.Select(x => x.GetTypeInfo().Name);
            logger.Information("Found these types: {DerivedTypes}", derivedTypesName);
            builder.RegisterTypes(derivedTypes).SingleInstance();

            //////
            // Localization Services
            //////
            builder.RegisterType<TreatmentMap>().As<ITreatmentMap>().SingleInstance();
            // This is a global sweep to find all types that implement IFieldRecordRegistration.  We then register every one of them.
            // Future would be to database this, but for now if it is referenced it is in.
            var myTypes = TypeHelper<ILocalizedStringResultTreatment>
                .FindTypesInAssemblies(TypeHelper<ILocalizedStringResultTreatment>.IsType);
            foreach (var type in myTypes)
            {
                builder.RegisterType(type).As<ILocalizedStringResultTreatment>();
            }
            builder.RegisterType<ResourceFetcher>().As<IResourceFetcher>();
            builder.RegisterType<P7RewriteMiddleware>();
        }
    }
}
