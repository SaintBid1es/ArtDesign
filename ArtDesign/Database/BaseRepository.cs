using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArtDesign.Model;

namespace ArtDesign.Database
{
    /// <summary>
    /// Универсальный базовый репозиторий, реализующий CRUD-операции для любой сущности, наследующей Base.
    /// </summary>
    /// <typeparam name="TEntity">Тип сущности (должен наследовать Base)</typeparam>
    public sealed class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : Base
    {
        private readonly ArtDesignDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public BaseRepository(ArtDesignDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        /// <summary>
        /// Получить объект по ID (без Include).
        /// Если не найдено, выбрасывается InvalidOperationException.
        /// </summary>
        public async Task<TEntity> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var entity = await _dbSet
                .FirstOrDefaultAsync(x => x.ID == id, cancellationToken);

            if (entity == null)
                throw new InvalidOperationException($"Entity {typeof(TEntity).Name} with id={id} not found.");

            return entity;
        }

        /// <summary>
        /// Получить объект по ID с подгрузкой связанных данных (Include).
        /// По умолчанию возвращает так же, как GetByIdAsync.
        /// Можно переопределить в наследниках, если нужно Include(...).
        /// </summary>
        public async Task<TEntity> GetByIdWithRelatedDataAsync(long id, CancellationToken cancellationToken = default)
        {
            // Можно вызывать некий protected метод IncludeRelatedData(_dbSet), если хотите гибко переопределять
            // но по умолчанию — без include, как GetByIdAsync
            return await _dbSet.FirstOrDefaultAsync(x => x.ID == id, cancellationToken);
        }

        /// <summary>
        /// Получить все объекты (без Include).
        /// </summary>
        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Получить все объекты с подгрузкой связанных данных.
        /// По умолчанию — без Include, можно переопределить в наследниках.
        /// </summary>
        public async Task<IEnumerable<TEntity>> GetAllWithRelatedDataAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Добавить новую сущность.
        /// Возвращает true, если добавление успешно.
        /// </summary>
        public async Task<bool> AddAsync(TEntity entity)
        {
            if (entity == null) return false;

            _dbSet.Add(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Полное обновление сущности.
        /// </summary>
        public async Task<bool> UpdateAsync(TEntity entity)
        {
            if (entity == null) return false;

            // Помечаем объект как Modified, чтобы EF обновил все поля
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Удаление по ID.
        /// Возвращает true, если удалось удалить.
        /// </summary>
        public async Task<bool> DeleteAsync(long id)
        {
            var existing = await _dbSet.FirstOrDefaultAsync(x => x.ID == id);
            if (existing == null) 
                return false;

            _dbSet.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Возвращает IQueryable для возможности гибкого запроса (Where, OrderBy и т.д.)
        /// </summary>
        public IQueryable<TEntity> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        /// <summary>
        /// Частичное обновление сущности: извлекаем сущность по ID, 
        /// выполняем произвольное действие updateAction, затем сохраняем.
        /// </summary>
        public async Task<bool> PartialUpdateAsync(long id, Action<TEntity> updateAction)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(x => x.ID == id);
            if (entity == null) return false;

            // Применяем переданный метод
            updateAction(entity);

            // Сохраняем изменения
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
