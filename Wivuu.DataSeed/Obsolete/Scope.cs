using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Wivuu.ExprUtils
{
    public class Scope : IDisposable
    {
        internal readonly Dictionary<string, ParameterExpression> Parameters =
            new Dictionary<string, ParameterExpression>();

        internal readonly Dictionary<string, ParameterExpression> Variables = 
            new Dictionary<string, ParameterExpression>();

        public ParameterExpression Param<T>(string ident)
        {
            var result = Expression.Parameter(typeof(T), ident);
            Parameters[ident] = result;
            return result;
        }

        public ParameterExpression Var<T>(string ident)
        {
            var result = Expression.Parameter(typeof(T), ident);
            Variables[ident] = result;
            return result;
        }

        public ParameterExpression Ref(string ident)
        {
            ParameterExpression result;
            return Parameters.TryGetValue(ident, out result) ? 
                result : Variables[ident];
        }

        private Expression Deref(Expression value)
        {
            var expect = typeof(Scope).GetMethod(nameof(Ref));

            return new CallbackVisitor(node =>
            {
                switch (node?.NodeType)
                {
                    case ExpressionType.Call:
                        var callExpr = node as MethodCallExpression;
                        if (callExpr.Method == expect)
                        {
                            var arg = callExpr.Arguments[0] as ConstantExpression;
                            return Ref(arg.Value as string);
                        }
                        break;
                }
                return node;
            }).Visit(value);
        }

        public Expression Expr(Expression<Action> value) => Deref(value.Body);
        public Expression Expr<T>(Expression<Func<T>> value) => Deref(value.Body);

        public void Dispose()
        {
        }
    }

    class CallbackVisitor : ExpressionVisitor
    {
        private readonly Func<Expression, Expression> OnVisit;

        public CallbackVisitor(Func<Expression, Expression> visit)
        {
            this.OnVisit = visit;
        }

        public override Expression Visit(Expression node)
        {
            return OnVisit(base.Visit(node));
        }
    }
}
