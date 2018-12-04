namespace P7.Core.Cache
{
    public class AutoObjectCache<TContaining, TObject> :
        ISingletonAutoObjectCache<TContaining, TObject>,
        IScopedAutoObjectCache<TContaining, TObject>
        where TContaining : class
        where TObject : class

    {
        private IAutoObjectCacheAllocator<TContaining, TObject> _allocator;
        

        public AutoObjectCache(IAutoObjectCacheAllocator<TContaining, TObject> allocator)
        {
            _allocator = allocator;
        }

        private TObject _value;

        public TObject Value
        {
            get
            {
                if (_value == null)
                {
                    _value = _allocator.Allocate();
                }

                return _value;
            }
            set { _value = value; }
        }
    }
}