using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Yalla.Parser.AstObjects;

namespace Yalla.Evaluator
{
    public class FuncFactory
    {
        public static object MakeFunc1(Type res, ProcedureNode proc, Environment env, Func<ProcedureNode, IList<object>, Environment, object> applyFunc)
        {
            Expression<Func<object>> funcExpr = () => applyFunc(proc, new List<object>(), env);

            var methodInfo = typeof(FuncFactory).GetMethod("MakeFunc1Generic", BindingFlags.Static | BindingFlags.NonPublic);

            return methodInfo.MakeGenericMethod(res).Invoke(null, new object[] { funcExpr });
        }

        public static object MakeFunc2(Type arg, Type res, ProcedureNode proc, Environment env, Func<ProcedureNode, IList<object>, Environment, object> applyFunc)
        {
            Expression<Func<object, object>> funcExpr = x => applyFunc(proc, new[] { x }, env);

            var methodInfo = typeof(FuncFactory).GetMethod("MakeFunc2Generic", BindingFlags.Static | BindingFlags.NonPublic);

            return methodInfo.MakeGenericMethod(arg, res).Invoke(null, new object[] { funcExpr });
        }

        public static object MakeFunc3(Type arg1, Type arg2, Type res, ProcedureNode proc, Environment env, Func<ProcedureNode, IList<object>, Environment, object> applyFunc)
        {
            Expression<Func<object, object, object>> funcExpr = (x, y) => applyFunc(proc, new[] { x, y }, env);

            var methodInfo = typeof(FuncFactory).GetMethod("MakeFunc3Generic", BindingFlags.Static | BindingFlags.NonPublic);

            return methodInfo.MakeGenericMethod(arg1, arg2, res).Invoke(null, new object[] { funcExpr });
        }

        // Used by reflection
        private static Func<TRes> MakeFunc1Generic<TRes>(Expression<Func<object>> func)
        {
            var converted = Expression.Convert(func.Body, typeof(TRes));

            return Expression.Lambda<Func<TRes>>(converted).Compile();
        }

        // Used by reflection
        private static Func<TArg, TRes> MakeFunc2Generic<TArg, TRes>(Expression<Func<object, object>> func)
        {
            var converted = Expression.Convert(func.Body, typeof(TRes));

            return Expression.Lambda<Func<TArg, TRes>>(converted, func.Parameters).Compile();
        }

        // Used by reflection
        private static Func<TArg1, TArg2, TRes> MakeFunc3Generic<TArg1, TArg2, TRes>(Expression<Func<object, object>> func)
        {
            var converted = Expression.Convert(func.Body, typeof(TRes));

            return Expression.Lambda<Func<TArg1, TArg2, TRes>>(converted, func.Parameters).Compile();
        }
    }
}
