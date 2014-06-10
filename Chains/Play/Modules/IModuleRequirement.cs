namespace Chains.Play.Modules
{
    public interface IModuleRequirement
    {
        bool CanExecute(object action);
    }
}
