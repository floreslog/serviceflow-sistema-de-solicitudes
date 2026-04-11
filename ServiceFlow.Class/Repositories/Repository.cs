using Microsoft.EntityFrameworkCore;
using ServiceFlow.Class.Data;
using ServiceFlow.Class.Models;

namespace ServiceFlow.Class.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly ServiceFlowDB db;
        protected readonly DbSet<T> dbSet;

        public Repository(ServiceFlowDB db)
        {
            this.db = db;
            dbSet = db.Set<T>();
        }

        public async Task<int> Create(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            dbSet.Add(entity);
            await db.SaveChangesAsync();
            return entity.Id;
        }

        public async Task Delete(int id)
        {
            var entity = await dbSet.FirstOrDefaultAsync(entity => entity.Id == id);
            if (entity == null) return;
            dbSet.Remove(entity);
            await db.SaveChangesAsync();
        }

        public virtual async Task<ICollection<T>> GetAll()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<T?> GetById(int id)
        {
            var entity = await dbSet.FirstOrDefaultAsync(entity => entity.Id == id);
            return entity;
        }

        public async Task Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (db.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Attach(entity);
            }
            dbSet.Update(entity);
            await db.SaveChangesAsync();
        }
    }
}
