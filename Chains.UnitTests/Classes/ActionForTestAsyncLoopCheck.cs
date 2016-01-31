namespace Chains.UnitTests.Classes
{
    using System.Threading.Tasks;

    public class ActionForTestAsyncLoopCheck: IChainableAction<ContextForTest, Task<ContextForTest>>
    {
        public readonly string ValueToCheckTo;

        public ActionForTestAsyncLoopCheck(string valueToCheckTo)
        {
            ValueToCheckTo = valueToCheckTo;
        }

        public Task<ContextForTest> Act(ContextForTest context)
        {
            if (ValueToCheckTo == context.ContextVariable)
            {
                context.HasBeenChecked = true;
                return Task.FromResult(context);
            }

            return Task.Delay(500).ContinueWith(x => context.Do(this)).Unwrap();
        }
    }
}
