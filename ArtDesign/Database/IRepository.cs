using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArtDesign.Model;

namespace ArtDesign.Database
{
    public interface IRepository<TEntity> where TEntity : Base
    {
        Task<TEntity> GetByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<TEntity> GetByIdWithRelatedDataAsync(long id, CancellationToken cancellationToken = default);

        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAllWithRelatedDataAsync(CancellationToken cancellationToken = default);

        Task<bool> AddAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(long id);

        /// <summary>
        /// Возвращает IQueryable, чтобы накладывать Where / OrderBy и т.п.
        /// </summary>
        IQueryable<TEntity> GetQueryable();

        /// <summary>
        /// Метод для частичного обновления, когда нужно, например, скачать объект и 
        /// применить к нему произвольное изменение, а потом сохранить.
        /// </summary>
        Task<bool> PartialUpdateAsync(long id, Action<TEntity> updateAction);
    }
}