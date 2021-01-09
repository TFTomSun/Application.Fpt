using System;

namespace Thomas.Apis.Core.New
{
    /// <summary>
    /// A base class for entities
    /// </summary>
    [New]
    public abstract class Entity : IEntity
    {
        /// <inheritdoc/>
        public Guid Id { get; set; }// = Guid.NewGuid();
    }
}