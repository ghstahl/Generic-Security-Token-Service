namespace P7.Core.Cache
{
    public interface ISingletonAutoObjectCache<TContaining, T> : IObjectCache<TContaining, T>
        where TContaining : class
        where T : class
    {
    }
}