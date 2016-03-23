namespace Chains.Play
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Chains.Exceptions;
    using Chains.Play.Modules;
    using Microsoft.CSharp.RuntimeBinder;

    public sealed class ExecuteActionAndGetResult : IChainableAction<ExecutionChain, ExecutionResultContext>
    {
        private readonly dynamic chainableAction;

        public ExecuteActionAndGetResult(dynamic chainableAction)
        {
            Check.ArgumentNull(chainableAction, nameof(chainableAction));

            this.chainableAction = chainableAction;
        }

        public ExecuteActionAndGetResult(string chainableActionTypeName, params object[] arguments)
        {
            Check.ArgumentNullOrEmpty(chainableActionTypeName, nameof(chainableActionTypeName));

            chainableAction = ExecutionChain.CreateObjectWithParameters(chainableActionTypeName, arguments);
        }

        public ExecutionResultContext Act(ExecutionChain context)
        {   
            object result = null;

            CheckRequirements(context.Modules, chainableAction);

            var modulesExtension = context.CurrentContext as IModular;
            if (modulesExtension != null)
            {
                CheckRequirements(modulesExtension.Modules, chainableAction);
            }

            try
            {
                result = context.CurrentContext.Do(chainableAction);
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
                            result = ((dynamic)module).Do(chainableAction);
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
                                result = ((dynamic)module).Do(chainableAction);
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

            return new ExecutionResultContext
                   {
                       LastExecutedAction = chainableAction,
                       Result = result
                   };
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
