using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

/// <summary>
/// Provides extenions method for enumerations.
/// </summary>
public static class EnumExtensions
{
	public static DirectoryInfo Directory(this Environment.SpecialFolder folder) => Environment.GetFolderPath(folder).ToDirectoryInfo();

	/// <summary>
	/// Gets a enum value combination of all values of a flag enum.
	/// </summary>
	/// <typeparam name="TEnum">The type of the flag enum.</typeparam>
	/// <param name="defaultEnumValue">The default value of the enum that invokes the extension method.</param>
	/// <returns>The combination of all values.</returns>
	public static TEnum All<TEnum>(this TEnum defaultEnumValue)
        where TEnum : struct, IComparable,  IFormattable
    {
        int combinedValue = 0;
        foreach(var enumValue in defaultEnumValue.GetValues())
        {
            var enumIntValue = (int) (object) enumValue;
            combinedValue |= enumIntValue;
        }
        return (TEnum) (Object) combinedValue;
    }
	/// <summary>
	/// Gets all values of the given enum type as a typesafe enumerable.
	/// <example>
	/// <code>
	/// var enumValues = default(MyEnum).GetValues();
	/// </code>
	/// </example>
	/// </summary>
	/// <typeparam name="TEnum">The type of the enum.</typeparam>
	/// <param name="defaultEnumValue">The caller of the extension method. Should be the default value of the enumeration.</param>
	/// <returns>All values of the given enum type as a typesafe enumerable.</returns>
	public static TEnum[] GetValues<TEnum>(this TEnum defaultEnumValue)
		where TEnum : struct, IComparable,  IFormattable
	{
		return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToArray();
	}

    /// <summary>
    /// Get generic IEnumerable flags of enum value
    /// </summary>
    /// <typeparam name="TEnum">Generic enum type</typeparam>
    /// <param name="flagEnumValue">Enum value</param>
    /// <returns>IEnumerable of enum flags</returns>
    public static IEnumerable<TEnum> Flags<TEnum>(this TEnum flagEnumValue)
  		where TEnum : struct, IComparable,  IFormattable
    {
        return default(TEnum).GetValues().Where(v => flagEnumValue.HasFlagGeneric(v));
    }

	/// <summary>
	/// Determines whether the given enum value contains the given flag value. This method should only be used for enums with the Flags attribute.
	/// </summary>
	/// <typeparam name="TEnum">The type of the enum.</typeparam>
	/// <param name="enumValue">The enum value that should be inspected, whether it contains the given flag value.</param>
	/// <param name="flag">The flag value.</param>
	/// <returns>true, if the given enum value contains the flag, otherwise false.</returns>
	public static bool HasFlagGeneric<TEnum>(this TEnum enumValue, TEnum flag)
		where TEnum : struct, IComparable,  IFormattable
	{
	    var baseValue = Convert.ToInt64(enumValue);
	    var flagValue = Convert.ToInt64(flag);
        
        var hasFlag = flagValue == 0 || (baseValue & flagValue) != 0;
		return hasFlag;
	}

	/// <summary>
	/// Determines whether the given enum value has a single instance of the given attribute type.
	/// </summary>
	/// <typeparam name="TAttribute">The type of the attribute.</typeparam>
	/// <typeparam name="TEnum">The type of the enum.</typeparam>
	/// <param name="enumValue">The enum value.</param>
	/// <param name="attribute">Out parameter. Returns the attribute instance, if one exists.</param>
	public static bool TryGetAttribute<TAttribute, TEnum>(this TEnum enumValue, out TAttribute attribute)
		where TAttribute : Attribute
		where TEnum : struct, IComparable,  IFormattable
	{
		attribute = typeof(TEnum).GetField(enumValue.ToString()).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
		return attribute != null;
	}

    /// <summary>
    /// Gets the attributes of the given enum value.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the searched attribute.</typeparam>
    /// <param name="enumValue">The enum value.</param>
    public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this Enum enumValue)
        where TAttribute : Attribute
    {
        var attributes = enumValue.GetType().GetField(enumValue.ToString()).GetCustomAttributes(false).OfType<TAttribute>();
        return attributes;
    }

	/// <summary>
	/// Gets the attributes of the given enum value.
	/// </summary>
	/// <typeparam name="TAttribute">The type of the searched attribute.</typeparam>
	/// <typeparam name="TEnum">The type of the enum.</typeparam>
	/// <param name="enumValue">The enum value.</param>
	/// <param name="attributes">Out parameter. Returns the attributes of the given attribute type.</param>
	public static void GetAttributes<TAttribute, TEnum>(this TEnum enumValue, out IEnumerable<TAttribute> attributes)
		where TAttribute : Attribute
		where TEnum : struct, IComparable,  IFormattable
	{
	    var x = (Enum)(Object)enumValue;
	    attributes = x.GetAttributes<TAttribute>();
	}

    /// <summary>
    /// Gets the attributes of the given enum value.
    /// </summary>
    /// <typeparam name="TAttribute">The type of the searched attribute.</typeparam>
    /// <param name="enumValue">The enum value.</param>
    /// <param name="attributes">Out parameter. Returns the attributes of the given attribute type.</param>
    public static void GetAttributes<TAttribute>(this Enum enumValue, out IEnumerable<TAttribute> attributes)
        where TAttribute : Attribute
    {
        attributes = enumValue.GetType().GetField(enumValue.ToString()).GetCustomAttributes(false).OfType<TAttribute>();
    }


	/// <summary>
	/// Converts the given enum value that is of another enum type than <typeparamref name="TEnum"/> to an enum instance
	/// of type <typeparamref name="TEnum"/>. The conversion will be performed using the enum value name, that means the specified enumValue has the
	/// same name like the returned value.
	/// </summary>
	/// <typeparam name="TEnum">The type of the returned enum value.</typeparam>
	/// <param name="enumValue">An enum value of another enum type.</param>
	/// <returns>The converted enum value.</returns>
	public static TEnum ToEnum<TEnum>(this Enum enumValue)
		where TEnum : struct,  IFormattable, IComparable
	{
		return (TEnum)Enum.Parse(typeof(TEnum), enumValue.ToString());
	}

    private static FieldInfo GetField<TEnum>(this TEnum value)
         where TEnum : struct,  IFormattable, IComparable
    {
        return typeof (TEnum).GetField(value.ToString());
    }

    /// <summary>
    /// Get description of enum
    /// </summary>
    /// <typeparam name="TEnum">Type of enum</typeparam>
    /// <param name="value">Value of Enum</param>
    /// <returns>Description string of current enum value</returns>
    public static String GetDescription<TEnum>(this TEnum value)
     where TEnum : struct,  IFormattable, IComparable
    {
        return value.GetField().GetDescriptionValue();//GetCustomAttributes<DescriptionAttribute>().Select(
            //da => da.Description).SingleOrDefault(d => d != null);
    }

   
  
}
