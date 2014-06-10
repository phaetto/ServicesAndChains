namespace Chains.Play.Security
{
    public interface IAuthorizableAction
    {
        string Session { get; set; }
    }
}
