namespace Chains.Play
{
    using System;
    using System.Collections.Generic;
    using Chains.Play.Modules;
    using System.Linq;
    using Chains.Exceptions;

    public sealed class ExecutionChain : Chain<ExecutionChain>, IModular
    {
        [ThreadStatic]
        private static Dictionary<string, Type> typeCache;

        public List<AbstractChain> Modules { get; set; }

        public dynamic CurrentContext { get; set; }

        public dynamic LastExecutedAction { get; set; }

        public ExecutionChain(dynamic currentContext)
        {
            Check.ArgumentNull(() => currentContext);

            CurrentContext = currentContext;

            Modules = new List<AbstractChain>();
        }

        public ExecutionChain(string currentContextTypeName)
        {
            Check.ArgumentNullOrEmpty(() => currentContextTypeName);

            CurrentContext = CreateObjectWithParameters(currentContextTypeName);

            Modules = new List<AbstractChain>();
        }

        public ExecutionChain(Type currentContextType)
        {
            Check.ArgumentNull(() => currentContextType);

            CurrentContext = CreateObjectWithParameters(currentContextType.AssemblyQualifiedName);

            Modules = new List<AbstractChain>();
        }

        public static object CreateObjectWithParameters(string unqualifiedType, params object[] parameters)
        {
            Check.ArgumentNullOrEmpty(() => unqualifiedType);

            return CreateObjectWithParametersAndInjection(unqualifiedType, parameters);
        }

        public static object CreateObjectWithParametersAndInjection(string unqualifiedType, object[] parameters, object[] injectedParameters = null)
        {
            Check.ArgumentNullOrEmpty(() => unqualifiedType);

            if (parameters == null)
            {
                parameters = new object[0];
            }

            var type = FindType(unqualifiedType);

            return CreateObjectWithParametersAndInjection(type, parameters, injectedParameters);
        }

        public static object CreateObjectWithParametersAndInjection(Type type, object[] parameters, object[] injectedParameters = null)
        {
            var constructors = type.GetConstructors().OrderByDescending(x => x.GetParameters().Length);
            foreach (var constructor in constructors)
            {
                try
                {
                    var constructorParameters = constructor.GetParameters();
                    var totalParametersCheck = parameters.Length
                        + (injectedParameters != null ? injectedParameters.Length : 0);

                    if (constructorParameters.Length <= totalParametersCheck)
                    {
                        // This is a candidate
                        var transformedObjects = new List<object>(totalParametersCheck);
                        for (var n = 0; n < parameters.Length; ++n)
                        {
                            transformedObjects.Add(Convert.ChangeType(parameters[n], constructorParameters[n].ParameterType));
                        }

                        if (injectedParameters != null)
                        {
                            for (var m = parameters.Length; m < constructorParameters.Length; ++m)
                            {
                                var found = false;
                                for (var n = 0; n < injectedParameters.Length; ++n)
                                {
                                    if (injectedParameters[n] != null
                                        && (constructorParameters[m].ParameterType.IsInstanceOfType(injectedParameters[n])
                                            || constructorParameters[m].ParameterType.IsSubclassOf(
                                                injectedParameters[n].GetType())))
                                    {
                                        transformedObjects.Add(
                                            Convert.ChangeType(injectedParameters[n], constructorParameters[m].ParameterType));
                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                {
                                    if (!constructorParameters[m].IsOptional)
                                    {
                                        throw new InvalidCastException();
                                    }

                                    transformedObjects.Add(constructorParameters[m].DefaultValue);
                                }
                            }
                        }

                        for (var n = totalParametersCheck; n < constructorParameters.Length; ++n)
                        {
                            if (!constructorParameters[n].IsOptional)
                            {
                                transformedObjects.Add(constructorParameters[n].DefaultValue);
                            }
                            else
                            {
                                transformedObjects.Add(null);
                            }
                        }

                        return constructor.Invoke(transformedObjects.ToArray());
                    }
                }
                catch (InvalidCastException)
                {
                    // Go to the next
                }
                //catch (Exception exception)
                //{
                //    if (exception.InnerException != null)
                //    {
                //        throw exception.InnerException;
                //    }

                //    throw;
                //}
            }

            throw new Exception("No constructor could be found to create the type: " + type.AssemblyQualifiedName);
        }

        public static Type FindType(string unqualifiedType)
        {
            Check.ArgumentNullOrEmpty(() => unqualifiedType);

            if (typeCache == null)
            {
                typeCache = new Dictionary<string, Type>();
            }

            if (typeCache.ContainsKey(unqualifiedType))
            {
                return typeCache[unqualifiedType];
            }

            var type = Type.GetType(unqualifiedType, false);
            if (type != null)
            {
                typeCache.Add(unqualifiedType, type);
                return type;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(unqualifiedType, false);
                if (type != null)
                {
                    typeCache.Add(unqualifiedType, type);
                    return type;
                }
            }

            throw new Exception("Type could not be found: " + unqualifiedType);
        }
    }
}
