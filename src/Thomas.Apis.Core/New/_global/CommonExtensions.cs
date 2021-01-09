using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Thomas.Apis.Core;

/// <summary>
    /// Provides common framework extensions.
    /// </summary>
    public static class CommonExtensions
    {
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="TSource"></typeparam>
        ///// <typeparam name="TResult"></typeparam>
        ///// <param name="sourceTask"></param>
        ///// <param name="asyncOperation"></param>
        ///// <returns></returns>
        //public static async Task<TResult> And<TSource, TResult>(this Task<TSource> sourceTask, Func<TSource, ValueTask<TResult>> asyncOperation)
        //{
        //    return await asyncOperation(await sourceTask);
        //}

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="sourceTask"></param>
    /// <param name="asyncOperation"></param>
    /// <returns></returns>
    public static async Task<TResult> And<TSource,TResult>(this Task<TSource> sourceTask, Func<TSource,Task<TResult>> asyncOperation)
        {
            return await asyncOperation(await sourceTask);
        }



        public static Delegate CreateDelegate(this MethodInfo methodInfo, object target = null)
        {
            Func<Type[], Type> getType;
            var isAction = methodInfo.ReturnType.Equals((typeof(void)));
            var types = methodInfo.GetParameters().Select(p => p.ParameterType);

            if (isAction)
            {
                getType = Expression.GetActionType;
            }
            else
            {
                getType = Expression.GetFuncType;
                types = types.Concat(new[] { methodInfo.ReturnType });
            }

            if (methodInfo.IsStatic)
            {
                return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);
            }

            return Delegate.CreateDelegate(getType(types.ToArray()), target.NullCheck("No target provided for instance method"), methodInfo.Name);
        }

    
        /// <summary>
        /// Determines whether the given type is static.
        /// </summary>
        /// <param name="typeInfo">The type to check</param>
        public static bool IsStatic(this TypeInfo typeInfo) => typeInfo.IsAbstract && typeInfo.IsSealed;
   
        /// <summary>
        /// Joins the given sequence of strings into one string.
        /// </summary>
        /// <param name="sequence">The string sequence to join.</param>
        /// <param name="separator">The separator between the joined strings.</param>
        /// <returns>The joined string.</returns>
        public static string Join(this IEnumerable<string> sequence, string separator = ", ")
        {
            return string.Join(separator, sequence);
        }
        /// <summary>
        /// Ensures that the give instance is not null. An <see cref="ApplicationException"/> will be thrown if it is null.
        /// </summary>
        /// <typeparam name="T">The type of the instance.</typeparam>
        /// <param name="instance">The instance to check</param>
        /// <param name="errorMessage">[optional] A custom error message.</param>
        /// <returns>The not null checked instance.</returns>
        public static T NotNull<T>(this T? instance, string errorMessage = null)
            where T: struct
        {
            if (instance == null)
            {
                throw new ApplicationException(errorMessage ?? $"The instance of type '{typeof(T).Name}' must not be null.");
            }
            return instance.Value;
        }

    /// <summary>
    /// Ensures that the give instance is not null. An <see cref="ApplicationException"/> will be thrown if it is null.
    /// </summary>
    /// <typeparam name="T">The type of the instance.</typeparam>
    /// <param name="instance">The instance to check</param>
    /// <param name="errorMessage">[optional] A custom error message.</param>
    /// <returns>The not null checked instance.</returns>
        public static T  NullCheck<T>( this  T instance, string errorMessage = null)
        {
            if(instance == null)
            {
                throw new ApplicationException(errorMessage ?? $"The instance of type '{typeof(T).Name}' must not be null.");
            }
            return instance;
        }

        /// <summary>
        /// Retrieves a substring from the the end of the given string.
        /// </summary>
        /// <param name="value">The string on which the extension is invoked.</param>
        /// <param name="startIndexFromEnd">[optional] The start index counted from the end of the string.</param>
        /// <param name="length">[optional] The length of the string. If not specifed the full string starting from given start index will be returned.</param>
        /// <returns>The substring.</returns>
        public static string SubstringReverse(this string value, int startIndexFromEnd = 0, int? length = null)
        {
            var lengthValue = length ?? value.Length - startIndexFromEnd;
            var startIndex = value.Length - startIndexFromEnd - lengthValue;
            return value.Substring(startIndex, lengthValue);
        }
    }

