using Connection.Data;
using Connection.models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection.models
{
    public interface ITenantIdProvider
    {
       public int TenantId {get;}
}
    public interface IGenericReadRepo<T> where T : class
    {
       Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
    }
    public interface IGenericWriteRepo<T> where T : class
    {
        Task<int> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
    }


    public interface IGenericRepo<T> : IGenericReadRepo<T>, IGenericWriteRepo<T>
      where T : class  // MUST repeat the constraint
    {
    }
    public class GenericRepo<T> : IGenericRepo<T> where T: class,IEntity
    {
        protected readonly SaasDashboardContext _context;
        protected readonly ILogger _logger;

        public GenericRepo(SaasDashboardContext context, ILogger<GenericRepo<T>> logger)
        {
            _context = context;
             _logger = logger;
        }

       virtual public  async  Task<IReadOnlyList<T>> GetAllAsync()
        {
            try
            {
                var dbSet = _context.Set<T>();
                return await dbSet.AsNoTracking().ToListAsync(); // You can add filters via expression
            }
            catch (Exception  ex)
            {
                _logger.LogError(ex, "Error fetching all {Entity}", typeof(T).Name);
                throw;
            }
        }

     virtual   public   async Task<T?> GetByIdAsync(int id)
        {
            try
            {
               // return await _context.Set<T>().FindAsync(id);
                return await _context.Set<T>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching {Entity} by Id {Id}", typeof(T).Name, id);
                throw;
            }
        }

        virtual public async Task<int> AddAsync(T entity)
        {
            try
            {
                
                await _context.Set<T>().AddAsync(entity);
                await _context.SaveChangesAsync();
                var idProperty = _context.Entry(entity).Property("Id").CurrentValue;

                return (int)idProperty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding {Entity}", typeof(T).Name);
                throw;

            }
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            try
            {
                _context.Set<T>().Update(entity);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating {Entity}", typeof(T).Name);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(T entity)
        {
            try
            {
                _context.Set<T>().Remove(entity);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting {Entity}", typeof(T).Name);
                return false;
            }
        }
    }


}
