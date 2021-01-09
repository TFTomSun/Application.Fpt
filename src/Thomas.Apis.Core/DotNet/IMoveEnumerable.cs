using System.Collections.Generic;

namespace Thomas.Apis.Core.DotNet
{
    /// <summary>
    /// A common interface for collections that implement their own move operation.
    /// </summary>
    /// <typeparam name="T">The type of the sequence elements.</typeparam>
    public interface IMoveEnumerable<T> : IEnumerable<T>
    {
        /// <summary>
        /// Moves the item to the specified index.
        /// </summary>
        /// <param name="item">The item to move.</param>
        /// <param name="index">The index where the item should be moved to.</param>
        void Move(T item, int index);
    }
}