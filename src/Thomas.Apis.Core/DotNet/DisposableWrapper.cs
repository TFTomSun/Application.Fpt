using System;

namespace Thomas.Apis.Core.DotNet
{
    /// <summary>
    /// Generic disposable wrapper
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DisposableWrapper<T> : IDisposableWrapper<T>
    {
        /// <summary>
        /// Generic item
        /// </summary>
        public T Item { get; private set; }

        private bool IsDisposed { get; set; }
        private Action<T> OnDispose { get; set; }

        /// <summary>
        /// Constructor for create generic disposable action
        /// </summary>
        /// <param name="item">Generic item</param>
        /// <param name="onDispose">Dispose action with generic parameter</param>
        public DisposableWrapper(T item, Action<T> onDispose)
        {
            Item = item;
            OnDispose = onDispose;
        }

        /// <summary>
        /// Implemented dispose function of IDisposable
        /// </summary>
        public void Dispose()
        {
            if (!IsDisposed && OnDispose != null)
            {
                OnDispose(Item);
                IsDisposed = true;
                OnDispose = null;
                Item = default;
            }
        }

        /// <summary>
        /// Get item of container
        /// </summary>
        /// <param name="container">Generic container</param>
        /// <returns>Item of container</returns>
        public static implicit operator T(DisposableWrapper<T> container)
        {
            return container.Item;
        }
    }
}
