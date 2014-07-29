namespace Services.Management.Administration.Executioner
{
    using System.Diagnostics;

    public sealed class ProcessExit : IProcessExit
    {
        public void Exit()
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
