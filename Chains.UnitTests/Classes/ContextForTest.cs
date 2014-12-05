namespace Chains.UnitTests.Classes
{
    public class ContextForTest : Chain<ContextForTest>
    {
        public string contextVariable = null;

        public bool HasBeenChecked = false;
    }
}
