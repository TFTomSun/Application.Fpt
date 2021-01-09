using System;

namespace Thomas.Apis.Core.DotNet
{
    /// <summary>
    /// Dynamic implementation of a disposable object.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class Disposable<TValue> : Disposable, IDisposable<TValue>
    {
        /// <summary>
        /// Dynamic value property.
        /// </summary>
        public TValue Value { get; }

        /// <summary>
        /// Dynamic dispoble for class.
        /// </summary>
        /// <param name="value">Value to use in invoked method.</param>
        /// <param name="onDispose">Method to invoke on dispose.</param>
        public Disposable(TValue value, Action onDispose) : base(onDispose)
        {
            Value = value;
        }
    }
    /// <summary>
    /// A simple implementation of the <see cref="IDisposable"/> interface that can be used, when you want to provide the using(...){...} pattern for 
    /// any workflow with a start / stop scenario.
    /// </summary>
    public class Disposable : IDisposable
    {
        private readonly Action m_onDispose;
        private bool m_isDisposed;

        /// <summary>
        /// Creates a new common Disposable instance.
        /// </summary>
        /// <param name="onDispose">The action that will be invoked, when the Disposable will be disposed.</param>
        public Disposable(Action onDispose)
        {
            m_onDispose = onDispose;
            m_isDisposed = false;
        }

        /// <summary>
        /// Invokes the specified dispose action.
        /// </summary>
        public void Dispose()
        {
            if (!m_isDisposed)
            {
                m_onDispose();
                m_isDisposed = true;
            }

        }
    }
}
