namespace Services.Management.UnitTests.Classes
{
    using System;
    using Chains;

    public class DisposableContextForTest : Chain<DisposableContextForTest>, IDisposable
    {
        public const string SuccessfullyStoppedMessage = "Disposed";

        public string contextVariable = null;

        public void Dispose()
        {
            contextVariable = SuccessfullyStoppedMessage;
        }
    }
}
