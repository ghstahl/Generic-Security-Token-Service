namespace P7.Core.Cache
{
    public class ObjectCacheAllocator<TContaining, TObject> : 
        IObjectCacheAllocator<TContaining, TObject>,
        IAutoObjectCacheAllocator<TContaining, TObject>
        where TContaining : class
        where TObject : class, new()
    {
        public TObject Allocate()
        {
            return new TObject();
        }
    }
}