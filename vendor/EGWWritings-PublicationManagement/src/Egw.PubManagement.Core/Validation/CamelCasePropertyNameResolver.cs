using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

using FluentValidation.Internal;
namespace Egw.PubManagement.Core.Validation;

/// <summary>
/// Camel case property name resolver
/// </summary>
public static class CamelCasePropertyNameResolver
{
    /// <summary>
    /// Resolves property name
    /// </summary>
    /// <param name="type"></param>
    /// <param name="memberInfo"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    public static string? ResolvePropertyName(Type type, MemberInfo memberInfo, LambdaExpression expression)
    {
        return ToCamelCase(DefaultPropertyNameResolver(memberInfo, expression));
    }

    private static string? DefaultPropertyNameResolver(MemberInfo? memberInfo, LambdaExpression? expression)
    {
        if (expression == null) return memberInfo != null ? memberInfo.Name : null;
        var chain = PropertyChain.FromExpression(expression);
        if (chain.Count > 0) return chain.ToString();

        return memberInfo != null ? memberInfo.Name : null;
    }

    // ReSharper disable once CognitiveComplexity
    private static string? ToCamelCase(string? s)
    {
        if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
        {
            return s;
        }

        char[] chars = s.ToCharArray();

        for (int i = 0; i < chars.Length; i++)
        {
            if (i == 1 && !char.IsUpper(chars[i]))
            {
                break;
            }

            bool hasNext = (i + 1 < chars.Length);
            if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
            {
                // if the next character is a space, which is not considered uppercase 
                // (otherwise we wouldn't be here...)
                // we want to ensure that the following:
                // 'FOO bar' is rewritten as 'foo bar', and not as 'foO bar'
                // The code was written in such a way that the first word in uppercase
                // ends when if finds an uppercase letter followed by a lowercase letter.
                // now a ' ' (space, (char)32) is considered not upper
                // but in that case we still want our current character to become lowercase
                if (!char.IsSeparator(chars[i + 1]))
                {
                    chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
                }

                break;
            }

            chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
        }

        return new string(chars);
    }
}