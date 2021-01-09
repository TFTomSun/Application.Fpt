using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Thomas.Apis.Core.New
{
    /// <summary>
    /// A common interface for CRUD services.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    [New]
    public interface ICrudService<TEntity>
    {
        /// <summary>
        /// Creates the given entity.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <returns></returns>
        Task<Guid> CreateAsync(TEntity entity);

        /// <summary>
        /// Updates the given entity.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns></returns>
        Task UpdateAsync(TEntity entity);

        /// <summary>
        /// Gets an entity
        /// </summary>
        /// <param name="id">The id of the entity to get.</param>
        /// <returns></returns>
        Task<TEntity> GetAsync(Guid id);

        /// <summary>
        /// Gets all entities
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TEntity>> GetAllAsync();

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="id">The id of the entity to delete.</param>
        /// <returns></returns>
        Task DeleteAsync(Guid id);
    }
}
