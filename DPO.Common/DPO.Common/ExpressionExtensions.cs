using System;
using System.Linq.Expressions;

namespace DPO.Common
{
    public static class ExpressionExtensions
    {
        public static string GetMemberName(this LambdaExpression expr)
        {
            var lexpr = expr;
            MemberExpression mexpr = null;
            if (lexpr.Body is MemberExpression)
            {
                mexpr = (MemberExpression)lexpr.Body;
            }
            else if (lexpr.Body is UnaryExpression)
            {
                mexpr = (MemberExpression)((UnaryExpression)lexpr.Body).Operand;
            }
            if (mexpr == null)
            {
                return null;
            }
            return mexpr.Member.Name;
        }

    }
}
