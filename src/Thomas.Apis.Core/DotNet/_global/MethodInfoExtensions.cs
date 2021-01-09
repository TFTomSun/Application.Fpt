
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

/// <summary>
/// Provides extension methods for the system reflection MethodInfo class.
/// </summary>
public static class MethodInfoExtensions
{

    /// <summary>
    /// A generic version of the .NET CreateDelegate method.
    /// </summary>
    /// <typeparam name="TDelegate">The type of the method delegate.</typeparam>
    /// <param name="method"></param>
    /// <returns></returns>
    public static TDelegate CreateDelegate<TDelegate>(this MethodInfo method)
    {
        return (TDelegate) (object) method.CreateDelegate(typeof (TDelegate));
    }

    /// <summary>
    /// Get the C# code signature of the method.
    /// </summary>
    /// <param name="method">The method info on which the extension is invoked.</param>
    /// <param name="callable">Return as an callable string(public void a(string b) would return a(b))</param>
    /// <param name="includeNamespaces">Should namespace be include. Default: false.</param>
    /// <param name="includeModifier">Determines whether the modifier (public, protected, etc.) are included.</param>
    /// <param name="includeMethodName">Determines whether the name of the method should be included in the created signature.</param>
    /// <param name="shortenNullable">Determines whether nullable parameters or return types shall be shortened according to the C# ? syntax.</param>
    /// <returns>The signature string.</returns>
    public static String Signature(this MethodInfo method, bool callable = false, bool includeNamespaces = false, bool includeModifier = true, bool includeMethodName = true, bool shortenNullable = true)
    {

        try
        {
            var firstParam = true;
            var sigBuilder = new StringBuilder();
            if (callable == false && includeModifier)
            {
                if (method.IsPublic)
                    sigBuilder.Append("public ");
                else if (method.IsPrivate)
                    sigBuilder.Append("private ");
                else if (method.IsAssembly)
                    sigBuilder.Append("internal ");
                if (method.IsFamily)
                    sigBuilder.Append("protected ");
                if (method.IsStatic)
                    sigBuilder.Append("static ");
                sigBuilder.Append(method.ReturnType.ClassName(includeNamespaces,shortenNullable: shortenNullable));
                sigBuilder.Append(' ');
            }

            if (includeMethodName)
            {
                sigBuilder.Append(method.Name);
            }

            // Add method generics
            if (method.IsGenericMethod)
            {
                sigBuilder.Append("<");
                foreach (var g in method.GetGenericArguments())
                {
                    if (firstParam)
                        firstParam = false;
                    else
                        sigBuilder.Append(", ");
                    sigBuilder.Append(g.ClassName(includeNamespaces, shortenNullable: shortenNullable));
                }
                sigBuilder.Append(">");
            }
            sigBuilder.Append("(");
            firstParam = true;
            var secondParam = false;
            foreach (var param in method.GetParameters())
            {
                if (firstParam)
                {
                    firstParam = false;
                    if (method.IsDefined(typeof (ExtensionAttribute), false))
                    {
                        if (callable)
                        {
                            secondParam = true;
                            continue;
                        }
                        sigBuilder.Append("this ");
                    }
                }
                else if (secondParam == true)
                    secondParam = false;
                else
                    sigBuilder.Append(", ");
                if (param.ParameterType.IsByRef)
                    sigBuilder.Append("ref ");
                else if (param.IsOut)
                    sigBuilder.Append("out ");
                if (!callable)
                {
                    sigBuilder.Append(param.ParameterType.ClassName(includeNamespaces,shortenNullable: shortenNullable));
                    sigBuilder.Append(' ');
                }
                sigBuilder.Append(param.Name);
            }
            sigBuilder.Append(")");
            return sigBuilder.ToString();
        }
        catch
        {
            return null;
        }
    }


    /// <summary>
    /// Determines whether the signatures of two method infos are equal.
    /// </summary>
    /// <param name="first">The method info on which the extension method is invoked.</param>
    /// <param name="second">The second method info.</param>
    /// <returns>true, when the signature is equal, otherwise false.</returns>
    public static bool HasSameSignature(this MethodInfo first, MethodInfo second, bool compareName =true)
    {
        first = first.IsGenericMethodDefinition || !first.IsGenericMethod ? first : first.GetGenericMethodDefinition();
        second = second.IsGenericMethodDefinition || !second.IsGenericMethod ? second :second.GetGenericMethodDefinition(); 

        var result = (!compareName || first.Name == second.Name) && first.ReturnType.IsLogicalEqualTo( second.ReturnType) && 
            first.GetParameters().Select(p => p.ParameterType).AreLogicalEqualTo(
                second.GetParameters().Select(p => p.ParameterType)) &&
                     first.GetGenericArguments().AreLogicalEqualTo(second.GetGenericArguments());
        return result;
    }

    


    /// <summary>
    /// Get interface definitions of an method
    /// </summary>
    /// <param name="method">Method to check</param>
    /// <param name="interfaceTypeSelector">How to check interface types</param>
    /// <returns>IEnumerable of interface method infos</returns>
    public static IEnumerable<MethodInfo> GetInterfaceDefinitions(this MethodInfo method,
        Func<Type, bool> interfaceTypeSelector = null)
    {
        if (interfaceTypeSelector == null)
        {
            interfaceTypeSelector = t => true;
        }
        return method.DeclaringType.GetInterfaces().Where(interfaceTypeSelector).Select(
            it => it.GetMethod(method.Name, method.GetParameters().Select(p => p.ParameterType).ToArray()))
            .Where(x=>x!=null);
    }
}