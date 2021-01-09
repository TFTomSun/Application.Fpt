using System;

namespace Thomas.Apis.Core.DotNet
{
    /// <summary>
    /// An disposable container that stores a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    public interface IDisposable<out TValue> : IDisposable
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        TValue Value { get; }
    }
}
