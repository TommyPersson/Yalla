using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Yalla.Parser.AstObjects;

namespace Yalla.Evaluator
{
    public class Applier
    {
        private readonly IDictionary<Type, Func<Applier, FunctionNode, IList<object>, Environment, object>> applyFunctions =
            new Dictionary<Type, Func<Applier, FunctionNode, IList<object>, Environment, object>>
                {
                    { typeof(PrimitiveFunction), (x, y, z, v) => x.Apply((PrimitiveFunction)y, z, v) },
                    { typeof(ProcedureNode), (x, y, z, v) => x.Apply((ProcedureNode)y, z, v) },
                    { typeof(NativeMethodFunctionNode), (x, y, z, v) => x.Apply((NativeMethodFunctionNode)y, z, v) },
                    { typeof(NativeConstructorFunctionNode), (x, y, z, v) => x.Apply((NativeConstructorFunctionNode)y, z, v) },
                    { typeof(NativeStaticMethodFunctionNode), (x, y, z, v) => x.Apply((NativeStaticMethodFunctionNode)y, z, v) },
                };
        
        private readonly IDictionary<string, Func<Applier, IList<object>, Environment, object>> primitiveFunctions =
            new Dictionary<string, Func<Applier, IList<object>, Environment, object>>
                {
                    { "+", (x, y, z) => x.ApplyAdd(y, z) },
                    { "=", (x, y, z) => x.ApplyEquals(y, z) },
                    { "<", (x, y, z) => x.ApplyLessThan(y, z) },
                    { "and", (x, y, z) => x.ApplyAnd(y, z) },
                    { "or", (x, y, z) => x.ApplyOr(y, z) },
                    { "cons", (x, y, z) => x.ApplyCons(y, z) },
                    { "lambda", (x, y, z) => x.ApplyLambda(y, z) },
                    { "def", (x, y, z) => x.ApplyDef(y, z) },
                    { "defmacro", (x, y, z) => x.ApplyDefMacro(y, z) },
                    { "set!", (x, y, z) => x.ApplySet(y, z) },
                    { "if", (x, y, z) => x.ApplyIf(y, z) },
                    { "let", (x, y, z) => x.ApplyLet(y, z) },
                    { "map", (x, y, z) => x.ApplyMap(y, z) },
                    { "make-func", (x, y, z) => x.ApplyMakeFunc(y, z)}
                };

        private readonly Evaluator evaluator;

        public Applier(Evaluator evaluator)
        {
            this.evaluator = evaluator;
        }

        public object Apply(FunctionNode function, IList<object> arguments)
        {
            return Apply(function, arguments, evaluator.GlobalEnvironment);
        }
        
        public object Apply(FunctionNode function, IList<object> arguments, Environment environment)
        {
            var result = applyFunctions[function.GetType()].Invoke(this, function, arguments, environment);
            
            return result;
        }

        public object Apply(PrimitiveFunction function, IList<object> arguments, Environment environment)
        {
            var result = primitiveFunctions[function.Symbol].Invoke(this, arguments, environment);
            
            return result;
        }
               
        public object Apply(ProcedureNode procedure, IList<object> arguments, Environment environment)
        {
            if (arguments.Count != procedure.Parameters.Count() && !procedure.Parameters.Select(x => x.Name).Contains("&"))
            {
                throw new ArgumentException("Wrong number of arguments given to procedure! Expected: " + procedure.Parameters.Count());
            }

            var localEnv = procedure.Environment.CreateChildEnvironment();
            
            for (int i = 0; i < procedure.Parameters.Count(); ++i)
            {
                object evaluatedArgument;
                bool finished = false;
                var parameterSymbol = procedure.Parameters.ElementAt(i);

                if (parameterSymbol.Name == "&")
                {
                    parameterSymbol = procedure.Parameters.ElementAt(i + 1);

                    evaluatedArgument =
                        procedure.IsMacro ? arguments.Skip(i).ToList()
                        : arguments.Skip(i).Select(arg => evaluator.Evaluate(arg, environment)).ToList();

                    localEnv.DefineSymbol(parameterSymbol, evaluatedArgument);

                    finished = true;
                }
                else
                {
                    evaluatedArgument =
                        procedure.IsMacro
                            ? arguments.ElementAt(i)
                            : evaluator.Evaluate(arguments.ElementAt(i), environment);
                }

                localEnv.DefineSymbol(parameterSymbol, evaluatedArgument);

                if (finished)
                {
                    break;
                }
            }

            var res = evaluator.Evaluate(procedure.Body, localEnv);

            if (procedure.IsMacro)
            {
                return evaluator.Evaluate(res, environment);
            }

            return res;
        }
        
        public object Apply(NativeMethodFunctionNode method, IList<object> arguments, Environment environment)
        {
            if (arguments.Count == 0)
            {
                throw new ArgumentException("No argument passed to native method call!");
            }

            var obj = evaluator.Evaluate(arguments.First(), environment);

            var args = arguments.Skip(1).Select(x => evaluator.Evaluate(x, environment)).ToList();
            var argTypes = args.Select(x => x.GetType()).ToList();

            Type otype = obj.GetType();
            var omethod = otype.GetMethod(method.Name, argTypes.ToArray());
            if (omethod != null)
            {
                return AstNode.MakeNode(omethod.Invoke(obj, args.ToArray()));
            }

            var oproperty = otype.GetProperty(method.Name);
            if (oproperty != null)
            {
                return AstNode.MakeNode(oproperty.GetValue(obj, null));
            }

            var ofield = otype.GetField(method.Name);
            if (ofield != null)
            {
                return AstNode.MakeNode(ofield.GetValue(obj));
            }

            throw new ArgumentException(method.Name + " is not a valid method, property or field on " + otype);
        }
        
        public object Apply(NativeConstructorFunctionNode constructor, IList<object> arguments, Environment environment)
        {
            var typeName = constructor.TypeName;

            var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType(typeName, false, false)).FirstOrDefault(x => x != null);

            if (type == null)
            {
                throw new ArgumentException("Type '" + typeName + "' not found!");
            }

            var args = arguments.Select(x => evaluator.Evaluate(x, environment)).ToArray();
            var argTypes = args.Select(x => x.GetType()).ToArray();

            object obj;

            try
            {
                obj = Activator.CreateInstance(type, args);
            }
            catch (Exception)
            {
                throw new ArgumentException("No constructor for '" + typeName + "' with argument types '" + argTypes.Select(x => x.Name).Aggregate((s1, s2) => s1 + "', '" + s2) + "'");
            }
            
            return AstNode.MakeNode(obj);
        }

        public object Apply(NativeStaticMethodFunctionNode staticMethod, IList<object> arguments, Environment environment)
        {
            var typeName = staticMethod.TypeName;
            var methodName = staticMethod.MethodName;

            var type = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType(typeName, false, false)).FirstOrDefault(x => x != null);

            if (type == null)
            {
                throw new ArgumentException("Type '" + typeName + "' not found!");
            }

            var args = arguments.Select(x => evaluator.Evaluate(x, environment)).ToArray();
            var argTypes = args.Select(x => x.GetType()).ToArray();
            
            try
            {
                var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static, null, argTypes, new ParameterModifier[0]);
                if (method != null)
                {
                    return AstNode.MakeNode(method.Invoke(null, args));
                }
                
                var property = type.GetProperty(methodName, BindingFlags.Public | BindingFlags.Static);
                if (property != null)
                {
                    return AstNode.MakeNode(property.GetValue(null, null));
                }

                var field = type.GetField(methodName, BindingFlags.Public | BindingFlags.Static);
                if (field != null)
                {
                    return AstNode.MakeNode(field.GetValue(null));
                }

                throw new ArgumentException("Static method '" + typeName + "/" + methodName + "' with argument types '" + argTypes.Select(x => x.Name).Aggregate((s1, s2) => s1 + "', '" + s2) + "' not found!");
            }
            catch (Exception)
            {
                throw new ArgumentException("Static method '" + typeName + "/" + methodName + "' with argument types '" + argTypes.Select(x => x.Name).Aggregate((s1, s2) => s1 + "', '" + s2) + "' not found!");
            }
        }
        
        public object ApplyAdd(IList<object> arguments, Environment environment)
        {
            var args = arguments.Select(x => evaluator.Evaluate(x, environment));

            bool returnInteger = args.All(x => x.GetType() == typeof(int));
            
            if (!returnInteger && !args.All(x => x.GetType() == typeof(int) || x.GetType() == typeof(decimal)))
            {
                throw new ArgumentException("'+' may only take integer or double values!");
            }

            decimal result = 0;

            foreach (var arg in args)
            {
                if (arg is int)
                {
                    result += (int)arg;
                }

                if (arg is decimal)
                {
                    result += (decimal)arg;
                }
            }

            if (returnInteger)
            {
                return (int)Convert.ToInt32(result);
            }
            
            return result;
        }

        public object ApplyAnd(IList<object> arguments, Environment environment)
        {
            foreach (var argument in arguments)
            {
                try
                {
                    var result = (bool)evaluator.Evaluate(argument, environment);

                    if (!result)
                    {
                        return false;
                    }
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException("Non-boolean value: " + argument);
                }
            }

            return true;
        }

        public object ApplyOr(IList<object> arguments, Environment environment)
        {
            foreach (var argument in arguments)
            {
                try
                {
                    var result = (bool)evaluator.Evaluate(argument, environment);

                    if (result)
                    {
                        return true;
                    }
                }
                catch (InvalidCastException)
                {
                    throw new ArgumentException("Non-boolean value: " + argument);
                }
            }

            return arguments.Count == 0;
        }

        public object ApplyEquals(IList<object> arguments, Environment environment)
        {
            var args = arguments.Select(x => evaluator.Evaluate(x, environment));

            var firstValue = args.First();
            var remainingValues = args.Skip(1);

            bool result = false;

            foreach (var value in remainingValues)
            {
                result = firstValue.Equals(value);
            }

            return AstNode.MakeNode(result);
        }
        
        public object ApplyLessThan(IList<object> arguments, Environment environment)
        {
            var args = arguments.Select(x => evaluator.Evaluate(x, environment));
            
            var left = args.ElementAt(0);
            var right = args.ElementAt(1);

            return IsLessThan(left, right);
        }

        public object ApplyCons(IList<object> arguments, Environment environment)
        {
            var args = arguments.Select(x => evaluator.Evaluate(x, environment)).ToList();

            if (args.Count != 2)
            {
                throw new ArgumentException("Cons expects 2 arguments, got " + args.Count);
            }

            var list = args.ElementAt(1) as IList<object>;
            if (list == null)
            {
                throw new ArgumentException("Second argument to cons must be a list.");
            }

            var newList = new List<object> { args.ElementAt(0) };
            newList.AddRange(list);

            return newList;
        }

        public object ApplyLambda(IList<object> arguments, Environment environment)
        {
            var parameterList = arguments.First() as IList<object>;
            var body = arguments.Skip(1);

            if (parameterList == null)
            {
                throw new ArgumentException("Missing parameter list in lambda!");
            }
            
            if (!parameterList.All(x => x is SymbolNode))
            {
                throw new ArgumentException("Parameters must be symbols!");
            }

            return new ProcedureNode(parameterList.Cast<SymbolNode>(), body, environment.CreateChildEnvironment());
        }

        public object ApplyDef(IList<object> arguments, Environment environment)
        {
            if (arguments.Count != 2)
            {
                throw new ArgumentException("Wrong number of arguments given to def! Expected: " + 2);
            }

            var symbol = arguments.ElementAt(0) as SymbolNode;

            if (symbol == null)
            {
                throw new ArgumentException("First argument to def not a symbol!");
            }

            var valueForm = arguments.ElementAt(1);

            var result = evaluator.Evaluate(valueForm, environment);

            environment.DefineSymbol(symbol, result);

            return symbol;
        }

        public object ApplyDefMacro(IList<object> arguments, Environment environment)
        {
            if (arguments.Count != 3)
            {
                throw new ArgumentException("Wrong number of arguments given to defmacro! Expected: " + 3);
            }

            var symbol = arguments.ElementAt(0) as SymbolNode;
            if (symbol == null)
            {
                throw new ArgumentException("First argument to defmacro not a symbol!");
            }

            var parameterList = arguments.ElementAt(1) as IList<object>;
            if (parameterList == null)
            {
                throw new ArgumentException("Second argument to defmacro not a parameterlist!");
            }

            var body = arguments.ElementAt(2);

            var macro = new ProcedureNode(parameterList.Cast<SymbolNode>(), new[] { body }, environment.CreateChildEnvironment(), true);
            
            environment.DefineSymbol(symbol, macro);

            return symbol;
        }

        public object ApplySet(IList<object> arguments, Environment environment)
        {
            /*if (arguments.Count != 2)
            {
                throw new ArgumentException("Wrong number of arguments given to set!! Expected: " + 2);
            }*/

            var symbol = arguments.ElementAt(0) as SymbolNode;
            if (symbol == null)
            {
                throw new ArgumentException("First argument to set! not a symbol!");
            }

            // (set! .Name cs-obj "Hello")
            if (symbol.Name.First() == '.')
            {
                var propName = symbol.Name.Substring(1);

                var propValue = evaluator.Evaluate(arguments.ElementAt(2), environment);
                var target = evaluator.Evaluate(arguments.ElementAt(1), environment);

                var otype = target.GetType();
                
                var oproperty = otype.GetProperty(propName);
                if (oproperty != null)
                {
                    oproperty.SetValue(target, propValue, null);
                    return AstNode.MakeNode(propValue);
                }

                var ofield = otype.GetField(propName);
                if (ofield != null)
                {
                    ofield.SetValue(target, propValue);
                    return AstNode.MakeNode(propValue);
                }

                throw new ArgumentException(propName + " is not a valid property or field of " + otype);
            }

            var value = evaluator.Evaluate(arguments.ElementAt(1), environment);

            if (!environment.SetSymbolValue(symbol, value))
            {
                throw new ArgumentException("Symbol '" + symbol.Name + "' not defined!");
            }

            return value;
        }
        
        public object ApplyIf(IList<object> arguments, Environment environment)
        {
            if (arguments.Count > 3 || arguments.Count < 2)
            {
                throw new ArgumentException("Wrong number of arguments given to if! Expected: 2 or 3");
            }

            var predicate = arguments.ElementAt(0);
            var successForm = arguments.ElementAt(1);

            var result = evaluator.Evaluate(predicate, environment);

            if (result.GetType() == typeof(NilNode) ||
                (result.GetType() == typeof(bool) && !((bool)result)))
            {
                return arguments.Count == 3 
                    ? evaluator.Evaluate(arguments.ElementAt(2), environment) 
                    : new NilNode();
            }

            return evaluator.Evaluate(successForm, environment);
        }

        public object ApplyLet(IList<object> arguments, Environment environment)
        {
            /*
             * (let ((var-a form-a) (var-b form-b)) (+ var-a var-b))
             */

            var bindingList = (IList<object>) arguments.First();
            var bodyforms = arguments.Skip(1).AsEnumerable();

            var localEnv = environment.CreateChildEnvironment();

            foreach (var binding in bindingList.OfType<IList<object>>())
            {
                var symbol = (SymbolNode)binding.First();
                var form = binding.ElementAt(1);

                var value = evaluator.Evaluate(form, localEnv);

                localEnv.DefineSymbol(symbol, value);
            }

            return evaluator.Evaluate(bodyforms, localEnv);
        }
                
        public object ApplyMap(IList<object> arguments, Environment environment)
        {
            /*
             * (map (lambda (x) (+ x 1)) '(1 2 3)) => (2 3 4)
             */
            
            var func = evaluator.Evaluate(arguments.First(), environment) as FunctionNode;
            var list = evaluator.Evaluate(arguments.ElementAt(1), environment) as IList<object>;

            if (func != null)
            {
                if (list != null)
                {
                    IList<object> result = list.Select(x => Apply(func, new List<object> { x }, environment)).ToList();

                    return result;
                }
                
                throw new ArgumentException("Second argument to map not a list!");
            }
            
            throw new ArgumentException("First argument to map not a function!");
        }

        private object ApplyMakeFunc(IList<object> objects, Environment environment)
        {
            /*
             * (make-func (list (get-list-type lst) System.Boolean) (lambda (x) (= (.Name x) "hej"))
             * */
            var types = ((IList<object>)evaluator.Evaluate(objects.First(), environment)).Cast<Type>();

            var proc = (ProcedureNode)evaluator.Evaluate(objects.ElementAt(1), environment);

            if (types.Count() == 2)
            {
                return MakeFunc2(types.ElementAt(0), types.ElementAt(1), proc, environment);
            }
            
            if (types.Count() == 3)
            {
                return MakeFunc3(types.ElementAt(0), types.ElementAt(1), types.ElementAt(2), proc, environment);
            }

            throw new ArgumentException("Could not create func of types " + types.Select(x => x.ToString()).Aggregate((s1, s2) => s1 + ", " + s2));
        }

        private static bool IsLessThan(object left, object right)
        {
            if (left.GetType() == typeof(int))
            {
                if (right.GetType() == typeof(int))
                {
                    return (int)left < (int)right;
                }
                
                if (right.GetType() == typeof(uint))
                {
                    return (int)left < (uint)right;
                }

                if (right.GetType() == typeof(long))
                {
                    return (int)left < (long)right;
                }

                if (right.GetType() == typeof(float))
                {
                    return (int)left < (float)right;
                }

                if (right.GetType() == typeof(decimal))
                {
                    return (int)left < (decimal)right;
                }
            }

            if (left.GetType() == typeof(uint))
            {
                if (right.GetType() == typeof(int))
                {
                    return (uint)left < (int)right;
                }

                if (right.GetType() == typeof(uint))
                {
                    return (uint)left < (uint)right;
                }

                if (right.GetType() == typeof(long))
                {
                    return (uint)left < (long)right;
                }

                if (right.GetType() == typeof(float))
                {
                    return (uint)left < (float)right;
                }

                if (right.GetType() == typeof(decimal))
                {
                    return (uint)left < (decimal)right;
                }
            }

            if (left.GetType() == typeof(long))
            {
                if (right.GetType() == typeof(int))
                {
                    return (long)left < (int)right;
                }

                if (right.GetType() == typeof(uint))
                {
                    return (long)left < (uint)right;
                }

                if (right.GetType() == typeof(long))
                {
                    return (long)left < (long)right;
                }

                if (right.GetType() == typeof(float))
                {
                    return (long)left < (float)right;
                }

                if (right.GetType() == typeof(decimal))
                {
                    return (long)left < (decimal)right;
                }
            }

            if (left.GetType() == typeof(float))
            {
                if (right.GetType() == typeof(int))
                {
                    return (float)left < (int)right;
                }

                if (right.GetType() == typeof(uint))
                {
                    return (float)left < (uint)right;
                }

                if (right.GetType() == typeof(long))
                {
                    return (float)left < (long)right;
                }

                if (right.GetType() == typeof(float))
                {
                    return (float)left < (float)right;
                }
            }
            
            if (left.GetType() == typeof(decimal))
            {
                if (right.GetType() == typeof(int))
                {
                    return (decimal)left < (int)right;
                }

                if (right.GetType() == typeof(uint))
                {
                    return (decimal)left < (uint)right;
                }

                if (right.GetType() == typeof(long))
                {
                    return (decimal)left < (long)right;
                }

                if (right.GetType() == typeof(decimal))
                {
                    return (decimal)left < (decimal)right;
                }
            }

            throw new ArgumentException("Not comparable types: " + left.GetType() + " and " + right.GetType());
        }

        private static Func<TArg, TRes> MakeFunc2Generic<TArg, TRes>(Expression<Func<object, object>> func)
        {
            var converted = Expression.Convert(func.Body, typeof(TRes));

            return Expression.Lambda<Func<TArg, TRes>>(converted, func.Parameters).Compile();
        }

        private static Func<TArg1, TArg2, TRes> MakeFunc3Generic<TArg1, TArg2, TRes>(Expression<Func<object, object>> func)
        {
            var converted = Expression.Convert(func.Body, typeof(TRes));

            return Expression.Lambda<Func<TArg1, TArg2, TRes>>(converted, func.Parameters).Compile();
        }

        private object MakeFunc2(Type arg, Type res, ProcedureNode proc, Environment env)
        {
            Expression<Func<object, object>> funcExpr = x => Apply(proc, new[] { x }, env);

            var methodInfo = GetType().GetMethod("MakeFunc2Generic", BindingFlags.Static | BindingFlags.NonPublic);

            return methodInfo.MakeGenericMethod(arg, res).Invoke(null, new object[] { funcExpr });
        }

        private object MakeFunc3(Type arg1, Type arg2, Type res, ProcedureNode proc, Environment env)
        {
            Expression<Func<object, object, object>> funcExpr = (x, y) => Apply(proc, new[] { x, y }, env);

            var methodInfo = GetType().GetMethod("MakeFunc3Generic", BindingFlags.Static | BindingFlags.NonPublic);

            return methodInfo.MakeGenericMethod(arg1, arg2, res).Invoke(null, new object[] { funcExpr });
        }
    }
}
