namespace P7.Core.Cache
{
    public interface IObjectCache<TContaining,T> where TContaining : class where T : class
    {
        T Value { get; set; }
    }
}