
using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

/// <summary>
/// Provides extension methods for linq expressions.
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    /// Gets the property info that belongs to the specified member expression (e.g. o=&gt;o.MyProperty)
    /// 
    /// </summary>
    /// <typeparam name="TSource">The type of the member source.</typeparam>
    /// <typeparam name="TValue">The type of the member value.</typeparam>
    /// <param name="memberExpression">The expression that represents the member access.</param>
    /// <returns>
    /// The member info.
    /// </returns>
    public static MemberInfo GetMemberInfo<TSource, TValue>(this Expression<Func<TSource, TValue>> memberExpression)
    {
        return memberExpression.GetMemberInfos().Last();
    }


    /// <summary>
    /// Gets the property info that belongs to the specified member expression (e.g. o=&gt;o.MyProperty)
    /// </summary>
    /// <param name="memberExpression">The expression that represents the member access.</param>
    /// <returns>The member info.</returns>
    public static MemberInfo GetMemberInfo(this LambdaExpression memberExpression)
    {
        return memberExpression.GetMemberInfos().Last();
    }

    /// <summary>
    /// Gets the property info that belongs to the specified member expression (e.g. o=&gt;o.MyProperty)
    /// </summary>
    /// <param name="memberExpression">The expression that represents the member access.</param>
    /// <returns>The member info.</returns>
    public static MemberInfo[] GetMemberInfos(this LambdaExpression memberExpression)
    {
        var memberInfos = GetMemberInfos(memberExpression.Body).ToArray();
        return memberInfos;
    }

    private static IEnumerable<MemberInfo> GetMemberInfos(Expression expression)
    {
        var memberExpression = expression as MemberExpression;
        if (memberExpression != null)
        {
            foreach (var outerMemberInfos in GetMemberInfos(memberExpression.Expression))
            {
                yield return outerMemberInfos;
            }
            yield return memberExpression.Member;

            yield break;
        }
        var methodCallExpression = expression as MethodCallExpression;
        if (methodCallExpression != null)
        {
            var baseExpression = methodCallExpression.Object;
            if (baseExpression == null && methodCallExpression.Method.IsExtensionMethod())
            {
                baseExpression = methodCallExpression.Arguments.First();
            }

                foreach (var outerMemberInfos in GetMemberInfos(baseExpression.NullCheck()))
            {
                yield return outerMemberInfos;
            }
            yield return methodCallExpression.Method;
            yield break;
        }
        var unaryExpression = expression as UnaryExpression;
        if (unaryExpression != null)
        {
            foreach (var outerMemberInfos in GetMemberInfos(unaryExpression.Operand))
            {
                yield return outerMemberInfos;
            }
        }
    }

}