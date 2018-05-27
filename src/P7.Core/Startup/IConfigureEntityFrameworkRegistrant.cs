using Microsoft.EntityFrameworkCore.Infrastructure;

namespace P7.Core.Startup
{
    public interface IConfigureEntityFrameworkRegistrant
    {
        void OnAddDbContext(EntityFrameworkServicesBuilder builder);
    }
}