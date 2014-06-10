namespace Chains.Play.Debug
{
    public sealed class DebugContext : Chain<DebugContext>
    {
        public DebugContext()
        {
        }

        public DebugContext(bool attachDebugger)
        {
            if (attachDebugger)
            {
                Do(new AttachDebugger());
            }
        }
    }
}
