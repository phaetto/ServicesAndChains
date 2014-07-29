namespace Services.Management.UnitTests.Classes
{
    using Services.Management.Administration.Executioner;

    internal class NoProcessExit : IProcessExit
    {
        public void Exit()
        {
            // Do not use process thingys on tests
        }
    }
}
