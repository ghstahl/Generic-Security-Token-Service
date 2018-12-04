namespace P7.Core.Cache
{
    public interface IAutoObjectCacheAllocator<TContaining, out T> where TContaining : class where T : class
    {
        T Allocate();
    }
}