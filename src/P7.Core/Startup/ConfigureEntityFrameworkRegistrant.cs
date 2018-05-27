using Microsoft.EntityFrameworkCore.Infrastructure;

namespace P7.Core.Startup
{
    public abstract class ConfigureEntityFrameworkRegistrant : IConfigureEntityFrameworkRegistrant
    {

        public abstract void OnAddDbContext(EntityFrameworkServicesBuilder builder);
    }
}