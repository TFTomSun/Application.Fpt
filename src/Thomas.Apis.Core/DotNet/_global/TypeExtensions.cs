
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using Thomas.Apis.Core;

/// <summary>
/// Provides Extension methods for the system type.
/// </summary>
// ReSharper disable once CheckNamespace
public static class TypeExtensions
{
    /// <summary>
    /// Determines whether the given value is the default value of its type.
    /// </summary>
    /// <param name="value">The object on which the method is invoked.</param>
    /// <typeparam name="T">The concrete type of the object.</typeparam>
    /// <returns>Whether the given object has the default value of its type.</returns>
    public static bool IsDefault<T>(this T value)
    {
        return EqualityComparer<T>.Default.Equals(value, default(T));
    }
    public static TSource As<TSource>(this TSource source)
    {
        return source;
    }
    public static TAttribute GetAttribute<TAttribute>(this Type type)
        where TAttribute : Attribute
    {
        return (TAttribute)type.GetCustomAttribute(typeof(TAttribute));
    }

    public static bool HasDefaultConstructor(this Type instance)
    {
        if (instance.IsGenericParameter)
            return instance.GetTypeInfo().GenericParameterAttributes.HasFlag((Enum)GenericParameterAttributes.DefaultConstructorConstraint);
        return instance.GetConstructors().Any(ci => ci.GetParameters().Length == 0);
    }

    public static FieldInfo GetAutoPropertyField(this Type type, string propertyName)
    {
        var allFields = type.To().Enumerable().Concat(type.BaseTypes()).SelectMany(t => t.GetRuntimeFields()).ToArray();
        var field = allFields.Single(
            a => Regex.IsMatch(a.Name, $"\\A<{propertyName}>k__BackingField\\Z"));

        return field;
    }

    /// <summary>
    /// Converts the given type to its nullable representant.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static Type ToNullableType(this Type type)
    {
        // Use Nullable.GetUnderlyingType() to remove the Nullable<T> wrapper if type is already nullable.
        if (type.IsValueType)
            return typeof (Nullable<>).MakeGenericType(type);
        else
            throw Api.Create.Exception($"The given type '{type.Name}' is no value type.");
    }

    private static IEqualityComparer<Type> TypeComparer { get; } = Api.Create.EqualityComparer<Type>(
        (t1, t2) => t1.IsLogicalEqualTo(t2), t => t.GetHashCode());

    /// <summary>
    /// Determines whether the given sequences of types are logical the same as the types in the second sequence.
    /// </summary>
    /// <param name="sequence">A sequence of types.</param>
    /// <param name="secondSequence">Another sequence of types.</param>
    /// <returns>true, if the type sequences are logical equal, otherwise false.</returns>
    public static bool AreLogicalEqualTo(this IEnumerable<Type> sequence, IEnumerable<Type> secondSequence)
    {

        return sequence.SequenceEqual(secondSequence, TypeComparer);
    }

    /// <summary>
    /// Determines whether the give type is logical the same as the second type.
    /// </summary>
    /// <param name="first">The first type.</param>
    /// <param name="second">The second type.</param>
    /// <returns>true, if the types are logical equal, otherwise false.</returns>
    public static bool IsLogicalEqualTo(this Type first, Type second)
    {
        if (first.IsGenericParameter && second.IsGenericParameter)
        {
            return first.GetGenericParameterConstraints().AreLogicalEqualTo(
                second.GetGenericParameterConstraints());
        }

        first = first.IsGenericTypeDefinition || !first.IsGenericType ? first : first.GetGenericTypeDefinition();
        second = second.IsGenericTypeDefinition || !second.IsGenericType ? second : second.GetGenericTypeDefinition();

        return first == second;
    }


    //public static IEnumerable<ITuple<Type,int>> InheritanceHierarchy(this Type type, Type other)
    //{
    //    yield return Api.Create.Tuple(type, 0);
    //    foreach (var baseTypeInfo in type.BaseTypes().Select((bt,i)=>Api.Create.Tuple(bt,i+1)))
    //    {
    //        yield return baseTypeInfo;
    //    }
    //    var interfaceTypes = type.GetInterfaces().ToList();
    //    interfaceTypes.Sort());
    //}
    /// <summary>
    /// determines weather the given type is a nullable enum type
    /// </summary>
    /// <param name="type">the type to check</param>
    /// <returns>weather it is a nullable enum type</returns>
    public static bool IsNullableEnum(this Type type)
    {
        var nullableType = Nullable.GetUnderlyingType(type);
        return (nullableType != null) && nullableType.IsEnum;
    }

    /// <summary>
    /// Gets the inner type of the specified type is a nullable. Otherwise null is returned
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>The inner nullable type.</returns>
    public static Type GetInnerNullableType(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));
        return Nullable.GetUnderlyingType(type);
    }

    /// <summary>
    /// determines weather the given type is a nullable type
    /// </summary>
    /// <param name="type">type to check</param>
    /// <returns>weather the type is nullable</returns>
    /// <exception cref="ArgumentNullException">when the type is null</exception>
    public static bool IsNullableType(this Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        var isNullable =  Nullable.GetUnderlyingType(type) != null;
        return isNullable;
    }

    /// <summary>
    /// Determines whether the specified type is an anonymous type.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns>true, if its an anonymous type, otherwise false.</returns>
    public static bool IsAnonymousType(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException("type");

        return Attribute.IsDefined(type, typeof (CompilerGeneratedAttribute), false)
               && type.IsGenericType && type.Name.Contains("AnonymousType")
               && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
               && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
    }


    /// <summary>
    /// Gets a deferred initialized sequence of the base types.
    /// </summary>
    /// <param name="type">The type on which the extension is invoked.</param>
    /// <returns>The base types.</returns>
    public static IEnumerable<Type> BaseTypes(this Type type)
    {
        var baseType = type.BaseType;
        while (baseType != null)
        {
            yield return baseType;
            baseType = baseType.BaseType;
        }
    }

    /// <summary>
    /// Get properties inclusiv base.
    /// </summary>
    /// <param name="type">Typoe to ger properties.</param>
    /// <returns>Enumerable of properties.</returns>
    public static IEnumerable<PropertyInfo> GetPropertiesInclBase(this Type type)
    {
        if (type.IsInterface)
        {
            return type.To().Enumerable().Concat(type.GetInterfaces()).Distinct().SelectMany(t => t.GetProperties());
        }
        return type.GetProperties();
    }

    /// <summary>
    /// Gets the default value of the specified type.
    /// </summary>
    /// <param name="type">The type on which the extension is invoked.</param>
    /// <returns>The default value of the type.</returns>
    public static object GetDefaultValue(this Type type)
    {
        if (type.IsValueType && type != typeof(void))
        {
            return Activator.CreateInstance(type);
        }
        return null;
    }

    /// <summary>
    /// Gets only the interfaces that are directly applied to the specified class or interface.
    /// </summary>
    /// <param name="type">The type of the class or interface from which the direct interfaces should be get.</param>
    /// <returns>The direct interfaces.</returns>
    public static IEnumerable<Type> GetDirectInterfaces(this Type type)
    {
        var allInterfaces = type.GetInterfaces();
        var directInterfaces = allInterfaces.Except(
            allInterfaces.SelectMany(t => t.GetInterfaces()));

        return directInterfaces;
    }

    /// <summary>
    /// Determines whether the specified type implements the given interface.
    /// </summary>
    /// <param name="type">The type on which the extension is invoked.</param>
    /// <param name="interfaceType">The interface type.</param>
    /// <returns>true, if the type implements the interface, otherwise false.</returns>
    public static bool Implements(this Type type, Type interfaceType)
    {
        var implementsInterface =  type.GetInterfaces().Any(t => t == interfaceType);
        return implementsInterface;
    }

    /// <summary>
    /// Determines whether the type is static.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsStatic(this Type type)
    {
        return type.IsAbstract && type.IsSealed;
    }

    /// <summary>
    /// Gets the extension methods that are defined in the specified type.
    /// </summary>
    /// <param name="type">The type on which the extension is invoked.</param>
    /// <param name="flags">The binding flags. Default: Only public extensions will be returned.</param>
    /// <returns>The extension methods.</returns>
    public static MethodInfo[] GetExtensionMethods(this Type type,
        BindingFlags flags = BindingFlags.Static | BindingFlags.Public)
    {
        return type.GetMethods(flags).Where(m => m.IsDefined(typeof (ExtensionAttribute), false)).ToArray();
    }

    /// <summary>
    /// Gets the C# class name of the type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="includeNamespace">Include namespace.</param>
    /// <param name="includeGenericParameterCount">Could generic parameter count included.</param>
    /// <param name="genericOpenCharacter">Generic open character</param>
    /// <param name="genericCloseCharacter">Generic close character.</param>
    /// <param name="shortenNullable">Determines whether nullable parameters or return types shall be shortened according to the C# ? syntax.</param>
    /// <returns>The class name.</returns>
    public static String ClassName(this Type type,  bool includeNamespace = true, 
        bool includeGenericParameterCount = false, char genericOpenCharacter = '<', char genericCloseCharacter = '>', bool shortenNullable =true)
    {
        var nullableType = Nullable.GetUnderlyingType(type);
        if (nullableType != null)
        {
            if (shortenNullable)
            {
                return nullableType.Name + "?";
            }
        }

        var typeName = string.IsNullOrWhiteSpace(type.FullName) || !includeNamespace ? type.Name : type.FullName;


        if (!type.IsGenericType)
            switch (type.Name)
            {
                case "String":
                    return "string";
                case "Int32":
                    return "int";
                case "Decimal":
                    return "decimal";
                case "Object":
                    return "object";
                case "Void":
                    return "void";
                default:
                {
                        return typeName;
                }
            }
        var genericTypeIndicator = '`';
        var genericSignIndex = typeName.IndexOf(genericTypeIndicator);
        if (genericSignIndex == -1)
        {
            return typeName;
        }
        var genericArguments = type.GetGenericArguments();
        var sb = new StringBuilder(typeName.Substring(0, genericSignIndex));
        if (includeGenericParameterCount)
        {
            sb.Append(genericTypeIndicator);
            sb.Append(genericArguments.Length);
        }
        sb.Append(genericOpenCharacter);
        var first = true;
        foreach (var t in genericArguments)
        {
            if (!first)
                sb.Append(',');
            sb.Append(t.ClassName(includeNamespace,includeGenericParameterCount,genericOpenCharacter,genericCloseCharacter,shortenNullable));
            first = false;
        }
        sb.Append(genericCloseCharacter);
        return sb.ToString();

        //string result;
        //if (type.IsGenericType)
        //{
        //    var genericArgumentString = type.GetGenericArguments().Select(t => t.ClassName()).Aggregate();
        //    result = String.Format("{0}<{1}>", type.Name.Substring(0, type.Name.Length - 2), genericArgumentString);
        //}
        //else
        //{
        //    result = type.Name;
        //}
        //return result;
    }

    /// <summary>
    /// Gets the custom attributes of the specified attribute type.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the attributes to get.</typeparam>
    /// <param name="type">The type on which the extension in invoked.</param>
    /// <param name="inherit">Determines whether the attribute declartions in base type should be considered.</param>
    /// <returns>The custom attributes.</returns>
    public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this Type type, bool inherit = false)
        where TAttribute : Attribute
    {
        return type.GetCustomAttributes(typeof (TAttribute), inherit).OfType<TAttribute>();
    }
}