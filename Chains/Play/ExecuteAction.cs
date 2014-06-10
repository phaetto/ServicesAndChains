namespace Chains.Play
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Chains.Play.Modules;
    using Microsoft.CSharp.RuntimeBinder;

    public sealed class ExecuteAction : IChainableAction<ExecutionChain, ExecutionChain>
    {
        private readonly dynamic chainableAction;

        public ExecuteAction(dynamic chainableAction)
        {
            if (chainableAction == null)
            {
                throw new ArgumentNullException("chainableAction");
            }

            this.chainableAction = chainableAction;
        }

        public ExecuteAction(string chainableActionTypeName, params object[] arguments)
        {
            if (string.IsNullOrEmpty(chainableActionTypeName))
            {
                throw new ArgumentNullException("chainableActionTypeName");
            }

            chainableAction = ExecutionChain.CreateObjectWithParameters(chainableActionTypeName, arguments);
        }

        public ExecutionChain Act(ExecutionChain context)
        {
            ApplyActionOnCurrentContext(context, chainableAction);

            context.LastExecutedAction = chainableAction;

            return context;
        }

        internal static void ApplyActionOnCurrentContext(ExecutionChain context, dynamic chainableAction)
        {
            CheckRequirements(context.Modules, chainableAction);

            var modulesExtension = context.CurrentContext as IModular;
            if (modulesExtension != null)
            {
                CheckRequirements(modulesExtension.Modules, chainableAction);
            }

            try
            {
                context.CurrentContext = context.CurrentContext.Do(chainableAction);
            }
            catch (RuntimeBinderException ex)
            {
                var hasBeenExecutedOnce = false;

                foreach (var module in context.Modules)
                {
                    var optionalModule = module as IOptionalModule;

                    if (optionalModule == null || optionalModule.IsEnabled)
                    {
                        try
                        {
                            ((dynamic)module).Do(chainableAction);
                            hasBeenExecutedOnce = true;
                        }
                        catch (RuntimeBinderException)
                        {
                        }
                    }
                }

                if (modulesExtension != null)
                {
                    foreach (var module in modulesExtension.Modules)
                    {
                        var optionalModule = module as IOptionalModule;

                        if (optionalModule == null || optionalModule.IsEnabled)
                        {
                            try
                            {
                                ((dynamic)module).Do(chainableAction);
                                hasBeenExecutedOnce = true;
                            }
                            catch (RuntimeBinderException)
                            {
                            }
                        }
                    }
                }

                if (!hasBeenExecutedOnce)
                {
                    throw new NotSupportedException(
                        "This type of action is not supported by context or modules (Tip: Only one implementation of IChainableAction<,> can be inferred automatically with ExecuteAction.)",
                        ex);
                }
            }
        }

        private static void CheckRequirements(IEnumerable<AbstractChain> modules, dynamic chainableAction)
        {
            foreach (var requirement in
                modules.Select(x => x as IModuleRequirement)
                       .Where(x => x != null)
                       .Where(requirement => !requirement.CanExecute(chainableAction)))
            {
                throw new InvalidOperationException(
                    requirement.GetType().FullName + " did not comply with the action requirements.");
            }
        }
    }
}
