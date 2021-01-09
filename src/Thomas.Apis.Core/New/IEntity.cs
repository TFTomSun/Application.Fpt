using System;

namespace Thomas.Apis.Core.New
{
    /// <summary>
    /// A common interface for entities with guid IDs.
    /// </summary>
    [New]
    public interface IEntity
    {
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        Guid Id { get; set; }
    }
}