using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Thomas.Apis.Core;
using Thomas.Apis.Core.DotNet;
using Thomas.Apis.Core.New;

/// <summary>
/// Provides access to types that are exposed by this assembly
/// </summary>
public static class FactoryExtensions
{
    [New]
    public static async Task StaTask<T>(this IFactory factory, Action action)
    {
        await factory.StaTask(() =>
        {
            action();
            return default(object);
        });
    }

    [New]
    public static Task<T> StaTask<T>(this IFactory factory, Func<T> function)
    {
        var tcs = new TaskCompletionSource<T>();
        Thread thread = new Thread(() =>
        {
            try
            {
                tcs.SetResult(function());
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        return tcs.Task;
    }

    /// <summary>
    /// Gets access to fluent conversion extensions.
    /// </summary>
    /// <typeparam name="T">The type of the instance.</typeparam>
    /// <param name="instance">The instance that shall be converted.</param>
    /// <returns>The entry point for fluent conversion extensions.</returns>
    public static IToContainer<T> To<T>(this T instance) => new ToContainer<T>(instance);

    /// <summary>
    /// Creates a new <see cref="IEqualityComparer{T}"/> from the given lambda expressions.
    /// </summary>
    /// <typeparam name="T">The type of the objects that should be compared.</typeparam>
    /// <param name="factory">The factory on which the extension is invoked.</param>
    /// <param name="compareFunction">A function that compares two instances of T.</param>
    /// <param name="getHashCode">A function that returns an hash code for an instance of T.</param>
    /// <returns>The equality comparer.</returns>
    public static IEqualityComparer<T> EqualityComparer<T>(this IFactory factory, Func<T, T, bool> compareFunction = null,
        Func<T, int> getHashCode = null)
    {
        if (compareFunction == null && getHashCode == null)
        {
            compareFunction = (a, b) => Equals(a, b);
            getHashCode = a => a.GetHashCode();
        }

        return new GenericEqualityComparer<T>(compareFunction, getHashCode);
    }

    /// <summary>
    /// Creates a wrapper for a value for which a dispose operation is required.
    /// </summary>
    /// <typeparam name="TItem">The type of the value.</typeparam>
    /// <param name="factory">The factory on which the extension is invoked.</param>
    /// <param name="wrappedItem">The wrapped value.</param>
    /// <param name="onDispose">The dispose action that will be invoked, when the wrapper is disposed.</param>
    /// <returns>The disposable wrapper.</returns>
    public static DisposableWrapper<TItem> DisposableWrapper<TItem>(this IFactory factory, TItem wrappedItem, Action<TItem> onDispose)
    {
        return new DisposableWrapper<TItem>(wrappedItem, onDispose);
    }

    /// <summary>
    /// Creates a new key value pair.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="factory">The factory on which the extension is invoked.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value .</param>
    /// <returns>The new key value pair.</returns>
    public static KeyValuePair<TKey, TValue> KeyValuePair<TKey,TValue>(this IFactory factory, TKey key, TValue value)
    {
        return new KeyValuePair<TKey, TValue>(key, value);
    }

    /// <summary>
    /// Creates a key value pair where the value is an object reference (not concrete type).
    /// </summary>
    /// <param name="factory">The factory on which the method is invoked.</param>
    /// <param name="key">The key.</param>
    /// <param name="value">The value (can be any type).</param>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <returns>The key value pair for key and value where the reference type of the value is object.</returns>
    public static KeyValuePair<TKey, object> ObjectKeyValuePair<TKey>(this IFactory factory, TKey key, object value)
    {
        return Api.Create.KeyValuePair(key, value);
    }

    /// <summary>
    /// Creates a new disposable.
    /// </summary>
    /// <param name="factory">The factory on which the extension is invoked.</param>
    /// <param name="onDispose">The action that should be invoked when the disposable is disposed.</param>
    /// <returns>The new disposable.</returns>
    public static IDisposable Disposable(this IFactory factory, Action onDispose)
    {
        return new Disposable(onDispose);
    }

    /// <summary>
    /// Creates a new dynamic disposable.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="factory">The factory on which the extension is invoked.</param>
    /// <param name="value">The value which will be invoked by on dispose</param>
    /// <param name="onDispose">The action that should be invoked when the disposable is disposed.</param>
    /// <returns>The new disposable.</returns>
    public static Disposable<TValue> Disposable<TValue>(this IFactory factory, TValue value, Action onDispose )
    {
        return new Disposable<TValue>(value, onDispose);
    }

   

    /// <summary>
    /// Creates a exception that provides error information for the specified aggregated exceptions.
    /// </summary>
    /// <param name="factory">The factory on which the extension is invoked.</param>
    /// <param name="aggregatedExceptions">The aggregated exceptions that should be bundled into one exception.</param>
    /// <param name="messagePattern">The message pattern for the aggregate exception.</param>
    /// <param name="parameters">The parameters of the message.</param>
    /// <returns>The aggregate exception.</returns>
    public static AggregateException AggregateException(this IFactory factory, IEnumerable<Exception> aggregatedExceptions,
        String messagePattern, params object[] parameters)
    {
        return factory.Exception((m, iex) => new AggregateException(m, aggregatedExceptions.ToArray()), null, messagePattern, parameters);
    }

    /// <summary>
    /// Creates a enumerable range
    /// </summary>
    /// <param name="factory">The factory on which the extension is invoked.</param>
    /// <param name="from">The start value of the range.</param>
    /// <param name="to">The end value from the range.</param>
    /// <returns></returns>
    public static IEnumerable<int> Range(this IFactory factory, int from, int to)
    {
        int increment = from < to ? 1 : -1;
        int counter = from;
        yield return from;
        while (counter != to)
        {
            counter += increment;
            yield return counter;
        }

    }

    /// <summary>
    /// Returns the passed lambda function. Might be necessary for automatic generic type resolution.
    /// </summary>
    /// <typeparam name="TResult">The type of the function result</typeparam>
    /// <param name="factory">The factory in which the extension method is invoked.</param>
    /// <param name="function">The function that will be returned.</param>
    /// <returns>The passed function.</returns>
    public static Func<TResult> Function<TResult>(this IFactory factory, Func<TResult> function)
    {
        return function;
    }

    /// <summary>
    /// Returns the passed lambda function. Might be necessary for automatic generic type resolution.
    /// </summary>
    /// <typeparam name="T1">The type of the first method parameter.</typeparam>
    /// <typeparam name="TResult">The type of the method result.</typeparam>
    /// <param name="factory">The factory on which the extension is invoked.</param>
    /// <param name="function">The lambda expression or method group that will be returned as func delegate.</param>
    /// <returns>The passed lambda expression or method group.</returns>
    public static Func<T1, TResult> Function<T1, TResult>(this IFactory factory, Func<T1,  TResult> function)
    {
        return function;
    }
    /// <summary>
    /// Returns the passed lambda function. Might be necessary for automatic generic type resolution.
    /// </summary>
    /// <typeparam name="T1">The type of the first method parameter.</typeparam>
    /// <typeparam name="T2">The type of the second method parameter.</typeparam>
    /// <typeparam name="TResult">The type of the method result.</typeparam>
    /// <param name="factory">The factory on which the extension is invoked.</param>
    /// <param name="function">The lambda expression or method group that will be returned as func delegate.</param>
    /// <returns>The passed lambda expression or method group.</returns>
    public static Func<T1, T2,TResult> Function<T1, T2,  TResult>(this IFactory factory, Func<T1, T2,  TResult> function)
    {
        return function;
    }

    /// <summary>
    /// Returns the passed lambda function. Might be necessary for automatic generic type resolution.
    /// </summary>
    /// <typeparam name="T1">The type of the first method parameter.</typeparam>
    /// <typeparam name="T2">The type of the second method parameter.</typeparam>
    /// <typeparam name="T3">The type of the third method parameter.</typeparam>
    /// <typeparam name="TResult">The type of the method result.</typeparam>
    /// <param name="factory">The factory on which the extension is invoked.</param>
    /// <param name="function">The lambda expression or method group that will be returned as func delegate.</param>
    /// <returns>The passed lambda expression or method group.</returns>
    public static Func<T1, T2, T3,  TResult> Function<T1, T2, T3,TResult>(this IFactory factory, Func<T1, T2, T3,TResult> function)
    {
        return function;
    }

    /// <summary>
    /// Returns the passed lambda function. Might be necessary for automatic generic type resolution.
    /// </summary>
    /// <typeparam name="T1">The type of the first method parameter.</typeparam>
    /// <typeparam name="T2">The type of the second method parameter.</typeparam>
    /// <typeparam name="T3">The type of the third method parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth method parameter.</typeparam>
    /// <typeparam name="TResult">The type of the method result.</typeparam>
    /// <param name="factory">The factory on which the extension is invoked.</param>
    /// <param name="function">The lambda expression or method group that will be returned as func delegate.</param>
    /// <returns>The passed lambda expression or method group.</returns>
    public static Func<T1,T2,T3,T4,TResult> Function<T1,T2,T3,T4,TResult>(this IFactory factory, Func<T1,T2,T3,T4,TResult> function)
    {
        return function;
    }

    /// <summary>
    /// Returns the passed lambda function. Might be necessary for automatic generic type resolution.
    /// </summary>
    /// <typeparam name="T1">The type of the first method parameter.</typeparam>
    /// <typeparam name="T2">The type of the second method parameter.</typeparam>
    /// <typeparam name="T3">The type of the third method parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth method parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth method parameter.</typeparam>
    /// <typeparam name="TResult">The type of the method result.</typeparam>
    /// <param name="factory">The factory on which the extension is invoked.</param>
    /// <param name="function">The lambda expression or method group that will be returned as func delegate.</param>
    /// <returns>The passed lambda expression or method group.</returns>
    public static Func<T1, T2, T3, T4, T5, TResult> Function<T1, T2, T3, T4, T5, TResult>(this IFactory factory, Func<T1, T2, T3, T4, T5, TResult> function)
    {
        return function;
    }


    /// <summary>
    /// Creates an action lambda expression.
    /// </summary>
    /// <param name="factory">The factory on which the extension is invoked.</param>
    /// <param name="expression">The action lambda expression.</param>
    /// <returns>The new action lambda expression.</returns>
    public static Expression<Action> ActionExpression (this IFactory factory, Expression<Action> expression)
    {
        return expression;
    }

    /// <summary>
    /// Returns the passed lambda function. Might be necessary for automatic generic type resolution.
    /// </summary>
    /// <param name="factory">The factory on which the extension is invoked.</param>
    /// <param name="action">The lambda expression or method group that will be returned as action delegate.</param>
    /// <returns>The passed lambda expression or method group.</returns>
    public static Action Action(this IFactory factory, Action action)
    {
        return action;
    }
    /// <summary>
    /// Returns the passed lambda function. Might be necessary for automatic generic type resolution.
    /// </summary>
    /// <typeparam name="T1">The type of the first method parameter.</typeparam>
    /// <param name="factory">The factory on which the extension is invoked.</param>
    /// <param name="action">The lambda expression or method group that will be returned as action delegate.</param>
    /// <returns>The passed lambda expression or method group.</returns>
    public static Action<T1> Action<T1>(this IFactory factory, Action<T1> action)
    {
        return action;
    }
    /// <summary>
    /// Returns the passed lambda function. Might be necessary for automatic generic type resolution.
    /// </summary>
    /// <typeparam name="T1">The type of the first method parameter.</typeparam>
    /// <typeparam name="T2">The type of the second method parameter.</typeparam>
    /// <param name="factory">The factory on which the extension is invoked.</param>
    /// <param name="action">The lambda expression or method group that will be returned as action delegate.</param>
    /// <returns>The passed lambda expression or method group.</returns>
    public static Action<T1, T2> Action<T1, T2>(this IFactory factory, Action<T1, T2> action)
    {
        return action;
    }
    /// <summary>
    /// Returns the passed lambda function. Might be necessary for automatic generic type resolution.
    /// </summary>
    /// <typeparam name="T1">The type of the first method parameter.</typeparam>
    /// <typeparam name="T2">The type of the second method parameter.</typeparam>
    /// <typeparam name="T3">The type of the third method parameter.</typeparam>
    /// <param name="factory">The factory on which the extension is invoked.</param>
    /// <param name="action">The lambda expression or method group that will be returned as func delegate.</param>
    /// <returns>The passed lambda expression or method group.</returns>
    public static Action<T1, T2, T3 > Action<T1, T2, T3>(this IFactory factory, Action<T1, T2, T3> action)
    {
        return action;
    }
    /// <summary>
    /// Returns the passed lambda function. Might be necessary for automatic generic type resolution.
    /// </summary>
    /// <typeparam name="T1">The type of the first method parameter.</typeparam>
    /// <typeparam name="T2">The type of the second method parameter.</typeparam>
    /// <typeparam name="T3">The type of the third method parameter.</typeparam>
    /// <typeparam name="T4">The type of the fourth method parameter.</typeparam>
    /// <typeparam name="T5">The type of the fifth method parameter.</typeparam>
    /// <param name="factory">The factory on which the extension is invoked.</param>
    /// <param name="action">The lambda expression or method group that will be returned as func delegate.</param>
    /// <returns>The passed lambda expression or method group.</returns>
    public static Action<T1, T2, T3, T4, T5> Action<T1, T2, T3, T4, T5>(this IFactory factory, Action<T1, T2, T3, T4, T5> action)
    {
        return action;
    }


    

    /// <summary>
    /// Extension method for creating an exception that matches to the caller.
    /// </summary>
    /// <param name="factory">The factory on which the extension method is invoked</param>
    /// <param name="messagePattern">The message pattern format. See String.Format for more information.</param>
    /// <param name="parameters">The parameters for the given message pattern.</param>
    /// <returns>The appropriate exception.</returns>
    public static Exception Exception(this IFactory factory, String messagePattern, params object[] parameters)
    {
        return factory.Exception(null, messagePattern, parameters);
    }

   

    /// <summary>
    /// Extension method for creating an exception that matches to the caller.
    /// </summary>
    /// <param name="factory">The factory on which the extension method is invoked</param>
    /// <param name="messagePattern">The message pattern format. See String.Format for more information.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="parameters">The parameters for the given message pattern.</param>
    /// <returns>The appropriate exception.</returns>
    public static Exception Exception(this IFactory factory, Exception innerException, String messagePattern, params object[] parameters)
    {
        return factory.Exception(
            (m,iex)=>new ApiException(m,iex,false),innerException, messagePattern, parameters);
    }

    /// <summary>
    /// Extension method for creating an exception that matches to the caller.
    /// </summary>
    /// <param name="messagePattern">The message pattern format. See String.Format for more information.</param>
    /// <param name="factory"></param>
    /// <param name="exceptionFactory">The factory that creates the new exception.</param>
    /// <param name="parameters">The parameters for the given message pattern.</param>
    /// <returns>The appropriate exception.</returns>
    public static TException Exception<TException>(this IFactory factory, Func<String, Exception, TException> exceptionFactory,
        String messagePattern, params object[] parameters)
        where TException : Exception
    {
        return factory.Exception(exceptionFactory, null, messagePattern, parameters);
    }


    /// <summary>
    /// Extension method for creating an exception that matches to the caller.
    /// </summary>
    /// <param name="messagePattern">The message pattern format. See String.Format for more information.</param>
    /// <param name="factory"></param>
    /// <param name="exceptionFactory">The factory that creates the new exception.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="parameters">The parameters for the given message pattern.</param>
    /// <returns>The appropriate exception.</returns>
    public static TException Exception<TException>(this IFactory factory, Func<String, Exception, TException> exceptionFactory,
        Exception innerException, String messagePattern, params object[] parameters)
        where TException : Exception
    {
        object[] parametersX = parameters.ToStringArray();
        var finalMessage = parameters.Length == 0 ? messagePattern : String.Format(messagePattern, parametersX);
        var exception = exceptionFactory(finalMessage, innerException);
        //Api.Global.Log().Screenshot("Screenshot for exception");
        //Api.Global.Log().Error(exception);
        return exception;
    }

  
}
