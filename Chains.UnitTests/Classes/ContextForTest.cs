namespace Chains.UnitTests.Classes
{
    public class ContextForTest : Chain<ContextForTest>
    {
        public string ContextVariable = null;

        public bool HasBeenChecked = false;
    }
}
