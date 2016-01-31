namespace Services.Management.UnitTests.Classes
{
    using System;
    using Chains;

    public class DisposableContextForTest : Chain<DisposableContextForTest>, IDisposable
    {
        public const string SuccessfullyStoppedMessage = "Disposed";

        public string ContextVariable = null;

        public void Dispose()
        {
            ContextVariable = SuccessfullyStoppedMessage;
        }
    }
}
