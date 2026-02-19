using Connection.models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface IGenericService<TDto>
    {
        Task<IReadOnlyList<TDto>> GetAllAsync(int dataKey);
        Task<TDto> GetByIdAsync(int id);
        Task<bool> AddAsync(TDto dto);
        Task<bool> UpdateAsync(TDto dto);
        Task<bool> DeleteAsync(int id);
    }
    public abstract class GenericService<TDto, TEntity> : IGenericService<TDto>
        where TEntity : class
    {
        private readonly IGenericRepo<TEntity> _repo;
        private readonly ILogger _logger;

        protected GenericService(IGenericRepo<TEntity> repo, ILogger logger)
        {
            _repo = repo;
            _logger = logger;
        }

        // Derived classes must implement mapping.
        protected abstract TDto ToDto(TEntity entity);
        protected abstract TEntity FromDto(TDto dto);

        public virtual async Task<IReadOnlyList<TDto>> GetAllAsync(int dataKey)
        {
            var list = await _repo.GetAllAsync(dataKey); // List<TEntity>
            return list.Select(ToDto).ToList();
        }

        public virtual async Task<TDto> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                throw new KeyNotFoundException($"{typeof(TEntity).Name} with Id {id} not found.");
            return ToDto(entity);
        }

        public virtual async Task<bool> AddAsync(TDto dto)
        {
            var entity = FromDto(dto); // mapping may parse strings -> DateTime
            return await _repo.AddAsync(entity);
        }

        public virtual async Task<bool> UpdateAsync(TDto dto)
        {
            var entity = FromDto(dto);
            // ensure exists
            var existing = await _repo.GetByIdAsync((int)entity.GetType().GetProperty("Id")!.GetValue(entity)!);
            if (existing == null)
            {
                _logger.LogWarning("{Entity} not found for update", typeof(TEntity).Name);
                return false;
            }
            return await _repo.UpdateAsync(entity);
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
            {
                _logger.LogWarning("{Entity} not found for delete Id {Id}", typeof(TEntity).Name, id);
                return false;
            }
            return await _repo.DeleteAsync(existing);
        }
    }
}
