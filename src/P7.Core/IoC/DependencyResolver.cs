using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace P7.Core.IoC
{
    public class DependencyResolver : IDependencyResolver
    {
        private IContainer container;
        private readonly ContainerBuilder builder;

        public DependencyResolver()
        {
            this.builder = new ContainerBuilder();
        }

        public void RegisterModule(IModule module)
        {
            this.builder.RegisterModule(module);
        }

        public void RegisterModules(IEnumerable<Assembly> assemblies)
        {
            this.builder.RegisterAssemblyModules(assemblies.ToArray());
        }

        public void Populate(IServiceCollection services)
        {
            this.builder.Populate(services);
        }

        public void Build()
        {
            this.container = this.builder.Build();
        }

        public T Resolve<T>() where T : class
        {
            return this.container?.Resolve<T>();
        }

        public IServiceProvider GetServiceProvider()
        {
            return this.container == null?null: new AutofacServiceProvider(this.container);
        }
    }
}
