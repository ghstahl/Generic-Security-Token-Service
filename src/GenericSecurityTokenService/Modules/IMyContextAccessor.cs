namespace GenericSecurityTokenService.Modules
{
    public interface IMyContextAccessor
    {
        MyContext MyContext { get; set; }
    }
}