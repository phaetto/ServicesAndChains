namespace Services.Management.Administration.Executioner
{
    public interface IExecutioner
    {
        void Execute();

        string GetProcessArguments(ExecutionMode mode);
    }
}
