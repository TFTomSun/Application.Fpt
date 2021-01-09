using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Thomas.Apis.Core.DotNet
{
    public class DisposableContainer : IDisposable
    {
        private readonly List<IDisposable> m_disposables = new List<IDisposable>(0);

        /// <summary>
        /// Disposable Container Constructor
        /// </summary>
        public DisposableContainer()
        { }

        /// <summary>
        /// Disposable container constructor with enumerable of disposables.
        /// </summary>
        /// <param name="disposables">Enumerable of disposables.</param>
        public DisposableContainer(IEnumerable<IDisposable> disposables)
        {
            this.m_disposables.AddRange(disposables);
        }

        private ConcurrentDictionary<string, IDisposable> Cache { get; } = new ConcurrentDictionary<string, IDisposable>();

        /// <summary>
        /// Get Lazy for disposable container.
        /// </summary>
        /// <typeparam name="T">Type of object.</typeparam>
        /// <param name="factory">Factory function.</param>
        /// <param name="propertyName">Property caller member name.</param>
        /// <returns>Lazy object.</returns>
        public T GetLazy<T>(Func<T> factory, [CallerMemberName] string propertyName = null)
            where T : IDisposable
        {
            var cachedValue = this.Cache.GetOrAdd(propertyName, k =>
            {
                var newValue = factory();
                this.m_disposables.Add(newValue);
                return newValue;
            });
            return (T)cachedValue;
        }

        /// <summary>
        /// Implement dispose function of interface
        /// </summary>
        public void Dispose()
        {
            var disposables = this.m_disposables.ToArray().Reverse().ToArray();

            this.m_disposables.Clear();
            this.Cache.Clear();

            disposables.ForEach(d => d?.Dispose(), true);
        }

        /// <summary>
        /// Clones the given disposable.
        /// </summary>
        /// <returns></returns>
        public DisposableContainer Clone()
        {
            var clone = new DisposableContainer();
            clone.Add(this.m_disposables);
            return clone;
        }

        /// <summary>
        /// Adds the disposable to the container.
        /// </summary>
        /// <param name="disposable">The disposable that should be disposed by the container.</param>
        public void Add(params IDisposable[] disposable)
        {
            this.m_disposables.AddRange(disposable);
        }

        /// <summary>
        /// Adds the disposable to the container.
        /// </summary>
        /// <param name="disposable">The disposable that should be disposed by the container.</param>
        public void Add(IEnumerable<IDisposable> disposable)
        {
            this.m_disposables.AddRange(disposable);
        }

        /// <summary>
        /// The operator is required to support the += operator.
        /// </summary>
        /// <param name="container">The disposable container on the left side of the += operator.</param>
        /// <param name="disposable">The disposable on the right side of the += operator.</param>
        /// <returns>The same disposable container.</returns>
        public static DisposableContainer operator +(DisposableContainer container, IDisposable disposable)
        {
            if (container == null)
            {
                container = new DisposableContainer();
            }
            container.Add(disposable);
            return container;
        }

        /// <summary>
        /// The operator is required to support the += operator.
        /// </summary>
        /// <param name="container">The disposable container on the left side of the += operator.</param>
        /// <param name="disposables">The disposables sequence on the right side of the += operator.</param>
        /// <returns>The same disposable container.</returns>
        public static DisposableContainer operator +(DisposableContainer container, IEnumerable<IDisposable> disposables)
        {
            if (container == null)
            {
                container = new DisposableContainer();
            }
            container.Add(disposables);
            return container;
        }

        /// <summary>
        /// The operator allows to attach actions to the disposable container.
        /// </summary>
        /// <param name="container">The disposable container on the left side of the += operator.</param>
        /// <param name="action">The lambda expression on the right side of the += operator.</param>
        /// <returns>The same disposable container.</returns>
        public static DisposableContainer operator +(DisposableContainer container, Action action)
        {
            container += Api.Create.Disposable(action);
            return container;
        }
    }
}