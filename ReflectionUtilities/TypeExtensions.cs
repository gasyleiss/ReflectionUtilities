using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ReflectionUtilities
{
    public static class TypeExtensions
    {
        private static MethodInfo GetMethod(Expression body)
        {
            MethodCallExpression method = body as MethodCallExpression;
            if (method != null) return method.Method;

            throw new ArgumentException("Not a method");
        }

        #region Invoke

        public static void Invoke<TType>(this TType obj, string name, params object[] parameters) where TType : class
        {
            MethodInfo mi = typeof (TType).GetInfo(name, parameters);
            if (mi != null)
            {
                mi.Invoke(obj, parameters);
                return;
            }

            throw new ArgumentException(string.Format("The member '{0}' is not a method.", name));
        }

        public static TResult Invoke<TType, TResult>(this TType obj, string name, params object[] parameters) where TType : class
        {
            MethodInfo mi = typeof (TType).GetInfo(name, parameters);
            if (mi != null)
                return (TResult) mi.Invoke(obj, parameters);

            throw new ArgumentException(string.Format("The member '{0}' is not a method.", name));
        }

        #endregion

        #region Name

        public static string GetName<TType>(this Type type, Expression<Action<TType>> action) where TType : class
        {
            return typeof (TType).GetInfo(action).Name;
        }

        public static string GetName<TType, TResult>(this Type type, Expression<Func<TType, TResult>> action) where TType : class
        {
            return typeof (TType).GetInfo(action).Name;
        }

        #endregion

        #region Info

        public static MethodInfo GetInfo<TType>(this Type type, Expression<Action<TType>> action) where TType : class
        {
            return GetMethod(action.Body);
        }

        public static MethodInfo GetInfo<TType, TResult>(this Type type, Expression<Func<TType, TResult>> action) where TType : class
        {
            return GetMethod(action.Body);
        }

        public static MethodInfo GetInfo(this Type type, string name, params object[] parameters)
        {
            if (parameters.Length == 0)
                return type.GetMethod(name, new Type[] {});

            Type[] types = parameters.Select(p => p.GetType()).ToArray();
            return type.GetMethod(name, types);
        }

        #endregion
    }
}