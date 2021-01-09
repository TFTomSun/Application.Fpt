using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

/// <summary>
/// Provides member info extensions
/// </summary>
public static class MemberInfoExtensions
{
    /// <summary>
    /// Determines whether the given method is an extension method.
    /// </summary>
    /// <param name="method">The method info.</param>
    /// <returns>true, if it is an extension method, otherwise false.</returns>
    public static bool IsExtensionMethod(this MethodInfo method)
    {
        var isExtension = method.IsDefined(typeof(ExtensionAttribute), true);
        return isExtension;
    }

    /// <summary>
    /// Gets the description attribute value of the member.
    /// </summary>
    /// <param name="memberInfo"></param>
    /// <returns></returns>
    public static string GetDescriptionValue(this MemberInfo memberInfo)
    {
        return memberInfo.GetCustomAttributes<DescriptionAttribute>().Select(
            da => da.Description).SingleOrDefault(d => d != null);
    }
    /// <summary>
    /// Gets the custom attributes of the specified attribute type.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attributes to get.</typeparam>
    /// <param name="memberInfo">The member info on which the extension in invoked.</param>
    /// <param name="inherit">Determines whether the attribute declarations in base implementations should be considered.</param>
    /// <returns>
    /// The custom attributes.
    /// </returns>
    public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this MemberInfo memberInfo, bool inherit = false)
            where TAttribute : Attribute
        {
            return memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).OfType<TAttribute>();
        }
    
    /// <summary>
    /// Gets the C# code name of the member. (excludes generic parameters, 
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static String MemberNameX(this MemberInfo method)
    {
        var methodName = method.Name;

        var offset = 0;

        var explicitImplementationDotIndex = methodName.LastIndexOf('.');
        if (explicitImplementationDotIndex !=-1)
        {
            methodName = '<'+methodName.Substring(explicitImplementationDotIndex+1);
        }
        if (methodName.StartsWith("<"))
        {
            offset += 1;
        }

        if (methodName.StartsWith("<set_") || methodName.StartsWith("<get_"))
        {
            offset += 4;
        }

        var length = methodName.Length;
        var closeTagIndex = methodName.LastIndexOf('>');
        if (closeTagIndex != -1)
        {
            length = closeTagIndex;
        }

        var memberName = methodName.Substring(offset, length - offset);

        var genericIndicatorIndex = memberName.IndexOf('`');
        if (genericIndicatorIndex!= -1)
        {
            memberName = memberName.Substring(0, genericIndicatorIndex);
        }

        return memberName;
    }
}

