using System;

namespace Thomas.Apis.Core.DotNet
{
    /// <summary>
    /// Generic disposable wrapper interface
    /// </summary>
    /// <typeparam name="T">Generic clas</typeparam>
    public interface IDisposableWrapper<out T> : IDisposable
    {
        /// <summary>
        /// Generic object
        /// </summary>
        T Item { get; }
    }
}