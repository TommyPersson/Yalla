using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Yalla.Parser.AstObjects;

namespace Yalla.Evaluator
{
    public class ActionFactory
    {
        public static object MakeAction0(ProcedureNode proc, Environment env, Func<ProcedureNode, IList<object>, Environment, object> applyFunc)
        {
            Expression<Action> actionExpr = () => applyFunc(proc, new List<object>(), env);

            return Expression.Lambda<Action>(actionExpr.Body).Compile();
        }

        public static object MakeAction1(Type arg, ProcedureNode proc, Environment env, Func<ProcedureNode, IList<object>, Environment, object> applyFunc)
        {
            Expression<Action<object>> actionExpr = x => applyFunc(proc, new[] { x }, env);

            var methodInfo = typeof(ActionFactory).GetMethod("MakeAction1Generic", BindingFlags.Static | BindingFlags.NonPublic);

            return methodInfo.MakeGenericMethod(arg).Invoke(null, new object[] { actionExpr });
        }

        public static object MakeAction2(Type arg1, Type arg2, ProcedureNode proc, Environment env, Func<ProcedureNode, IList<object>, Environment, object> applyFunc)
        {
            Expression<Action<object, object>> actionExpr = (x, y) => applyFunc(proc, new[] { x, y }, env);
            
            var methodInfo = typeof(ActionFactory).GetMethod("MakeAction2Generic", BindingFlags.Static | BindingFlags.NonPublic);

            return methodInfo.MakeGenericMethod(arg1, arg2).Invoke(null, new object[] { actionExpr });
        }

        // Used by reflection
        private static Action<TArg> MakeAction1Generic<TArg>(Expression<Action<object>> action)
        {
            return Expression.Lambda<Action<TArg>>(action.Body, action.Parameters).Compile();
        }

        // Used by reflection
        private static Action<TArg1, TArg2> MakeAction2Generic<TArg1, TArg2>(Expression<Action<object, object>> action)
        {
            return Expression.Lambda<Action<TArg1, TArg2>>(action.Body, action.Parameters).Compile();
        }
    }
}
