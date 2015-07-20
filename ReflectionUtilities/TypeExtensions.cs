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
                if (!mi.IsGenericMethod)
                    mi.Invoke(obj, parameters);
                else
                {
                    Type[] types = parameters.Select(p => p.GetType()).ToArray();
                    MethodInfo generic = mi.MakeGenericMethod(types);
                    generic.Invoke(obj, parameters);
                }
                return;
            }

            throw new ArgumentException(string.Format("The member '{0}' is not a method.", name));
        }

        public static TResult Invoke<TType, TResult>(this TType obj, string name, params object[] parameters) where TType : class
        {
            MethodInfo mi = typeof (TType).GetInfo(name, parameters);
            if (mi != null)
                if (!mi.IsGenericMethod)
                    return (TResult) mi.Invoke(obj, parameters);
                else
                {
                    Type[] types = parameters.Select(p => p.GetType()).ToArray();
                    MethodInfo generic = mi.MakeGenericMethod(types);
                    return (TResult) generic.Invoke(obj, parameters);
                }

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
            MethodInfo result = null;

            #region non-generic

            #region w/o parameters

            if (parameters.Length == 0)
            {
                result = type.GetMethod(name, new Type[] {});
                if (result != null)
                    return result;
            }

            #endregion

            #region w/ parameters

            Type[] types = parameters.Select(p => p.GetType()).ToArray();
            result = type.GetMethod(name, types);

            if (result != null) return result;

            #endregion

            #endregion

            #region generic

            MethodInfo method = type.GetMember(name)
                .Where(info => info.MemberType == MemberTypes.Method
                               && ((MethodInfo) info).IsGenericMethod
                    // for generic method check only the number of parameters --> it should be equal to method generic parameters
                               && ((MethodInfo) info).GetParameters().Length == types.Length)
                .Select(info => (MethodInfo) info)
                .SingleOrDefault();

            return method;

            #endregion
        }

        #endregion
    }
}