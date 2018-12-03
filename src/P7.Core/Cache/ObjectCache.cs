namespace P7.Core.Cache
{
    public class ObjectCache<TContaining, TObject> :
        ISingletonObjectCache<TContaining, TObject>,
        IScopedObjectCache<TContaining, TObject>

        where TContaining : class
        where TObject : class
    {
        public TObject Value { get; set; }
    }
}