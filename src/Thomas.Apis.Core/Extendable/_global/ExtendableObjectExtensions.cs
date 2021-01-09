
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Thomas.Apis.Core.Extendable;

/// <summary>
/// Provides extension methods for the <see cref="IExtendableObject"/> interface.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ExtendableObjectExtensions
{

    /// <summary>
    /// Sets the value of a extension property.
    /// </summary>
    /// <typeparam name="TMember">The type of the extension property.</typeparam>
    /// <param name="dynamicObject">The dynamic object on which the extension is invoked.</param>
    /// <param name="value">The value that should be set.</param>
    /// <param name="memberName">The name / key of the member.</param>
    public static void Set<TMember>(this IExtendableObject dynamicObject, TMember value, [CallerMemberName] string memberName = null)
    {
        dynamicObject.SetMemberByName(value, memberName.NullCheck());
    }




    /// <summary>
    /// Clears the member cache of the <see cref="IExtendableObject"/> instance.
    /// </summary>
    /// <param name="dynamicObject">The <see cref="IExtendableObject"/> instance on which the extension method is invoked.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ClearCache(this IExtendableObject dynamicObject)
    {
        dynamicObject.Cache.Clear();
    }



    /// <summary>
    /// The first time when the function is invoked for a specific member, the member value is evaluated. Further calls will return always the same value.
    /// </summary>
    /// <typeparam name="TMember">The type of the member.</typeparam>
    /// <param name="dynamicObject">The extendable object on which the extension is invoked.</param>
    /// <param name="getInitialValue">A function that returns the initial member value.</param>
    /// <param name="memberNameOrKey">When specified, overrides the member name of the calling function that is used as a key for the value cache.</param>
    /// <param name="initializeWhenDefault">Determines whether the value is reevaluated as long as the previous evaluation returned the default of the value type.</param>
    /// <param name="forceRefresh">Determines whether the value should be reevaluated again, even if its already initialized.</param>
    /// <param name="onInitialized">A hook that can be specified, to determine whether the value was initialized and/or to modify the value before it is stored.</param>
    /// <returns>The member value.</returns>
    public static TMember Get<TMember>(this IExtendableObject dynamicObject, Func<TMember> getInitialValue,
         [CallerMemberName] string memberNameOrKey = null, bool initializeWhenDefault = false, bool forceRefresh = false, Action<TMember> onInitialized = null)
    {
        if (memberNameOrKey == null) throw new ArgumentNullException(nameof(memberNameOrKey));
        var memberName = memberNameOrKey;

        if (forceRefresh || !dynamicObject.Cache.TryGetValue(memberName, out var member) ||
            initializeWhenDefault && Equals(member, default(TMember)))
        {
            lock (dynamicObject)
            {
                if (forceRefresh || !dynamicObject.Cache.TryGetValue(memberName, out member) ||
                    initializeWhenDefault && Equals(member, default(TMember)))
                {
                    member = getInitialValue == null ? default(TMember) : getInitialValue();

                    if (onInitialized != null)
                    {
#pragma warning disable CS8604 // Possible null reference argument.
                        onInitialized((TMember) member);
#pragma warning restore CS8604 // Possible null reference argument.
                    }

                    var cache = dynamicObject.Cache;
                    if (cache.ContainsKey(memberName))
                    {
                        cache[memberName] = member;
                    }
                    else
                    {
                        cache.Add(memberName, member);
                    }
                }
            }
        }

#pragma warning disable CS8603 // Possible null reference return.
        return (TMember)member;
#pragma warning restore CS8603 // Possible null reference return.
    }




    /// <summary>
    /// Determines whether a member with the specified name is already available in the dynamic object cache.
    /// </summary>
    /// <param name="dynamicObject">The dynamic object on which the extension is invoked.</param>
    /// <param name="memberName">The name / key of the member to look for.</param>
    /// <returns>true, if a member entry exists, otherwise false.</returns>
    public static bool HasMember(this IExtendableObject dynamicObject, String memberName)
    {
        var hasMember = dynamicObject.Cache.ContainsKey(memberName);
        return hasMember;
    }


    private static void SetMemberByName<TMember>(this IExtendableObject dynamicObject, TMember value, string memberName)
    {
        lock (dynamicObject)
        {

            if (dynamicObject.Cache.ContainsKey(memberName))
            {
               
                dynamicObject.Cache[memberName] = value;
            }
            else
            {
                dynamicObject.Cache.Add(memberName, value);
            }
        }

    }

}