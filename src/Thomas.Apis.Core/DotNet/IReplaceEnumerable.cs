using System.Collections.Generic;

namespace Thomas.Apis.Core.DotNet
{
    /// <summary>
    /// A common interface for collections that implement their own replace operation.
    /// </summary>
    /// <typeparam name="T">The type of the sequence elements.</typeparam>
    public interface IReplaceEnumerable<T> : IEnumerable<T>
    {
        /// <summary>
        /// Replaces the old item with the new item.
        /// </summary>
        /// <param name="oldItem">The old item to replace.</param>
        /// <param name="newItem">The new item.</param>
        void Replace(T oldItem, T newItem);

        /// <summary>
        /// Replaces the item at the specfied index with the new item.
        /// </summary>
        /// <param name="index">The index of the item to be replaced.</param>
        /// <param name="newItem">The new item.</param>
        void ReplaceAt(int index, T newItem);
    }
}