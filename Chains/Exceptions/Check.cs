namespace Chains.Exceptions
{
    using System;
    using System.Linq.Expressions;

    public static class Check
    {
        public static void ArgumentNull<T>(Expression<Func<T>> action, string message = null)
        {
            var testObject = action.Compile()();

            if (!Equals(testObject, null))
            {
                return;
            }

            var expression = (MemberExpression)action.Body;
            var name = expression.Member.Name;
            throw new ArgumentNullException(name, message ?? $"Parameter '{name}' is null");
        }

        public static void ArgumentNullOrEmpty(Expression<Func<string>> action, string message = null)
        {
            var testObject = action.Compile()();

            if (!string.IsNullOrEmpty(testObject))
            {
                return;
            }

            var expression = (MemberExpression)action.Body;
            var name = expression.Member.Name;
            throw new ArgumentNullException(name, message ?? $"Parameter '{name}' is null or empty");
        }

        public static void Argument<T>(bool condition, Expression<Func<T>> action, string message)
        {
            if (!condition)
            {
                return;
            }

            var expression = (MemberExpression)action.Body;
            var name = expression.Member.Name;
            throw new ArgumentException($"Argument '{name}' error: {message}", name);
        }

        public static void ArgumentOutOfRange<T>(bool condition, Expression<Func<T>> action, string message)
        {
            if (!condition)
            {
                return;
            }

            var actualValue = action.Compile()();

            var expression = (MemberExpression)action.Body;
            var name = expression.Member.Name;
            throw new ArgumentOutOfRangeException(name, actualValue, $"Argument out of range '{name}': {message}");
        }

        public static void ConditionNotSupported(bool condition, string message, Exception baseException = null)
        {
            if (!condition)
            {
                return;
            }

            throw new NotSupportedException(message, baseException);
        }
    }
}
