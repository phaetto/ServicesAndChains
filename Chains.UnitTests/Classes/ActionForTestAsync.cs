namespace Chains.UnitTests.Classes
{
    using System.Threading.Tasks;

    public class ActionForTestAsync: IChainableAction<ContextForTest, Task<ContextForTest>>
    {
        public readonly string ValueToChangeTo;

        public ActionForTestAsync(string valueToChangeTo)
        {
            ValueToChangeTo = valueToChangeTo;
        }

        public Task<ContextForTest> Act(ContextForTest context)
        {
            return Task.Delay(300).ContinueWith(
                x =>
                {
                    context.contextVariable = ValueToChangeTo;

                    return context;
                });
        }
    }
}
