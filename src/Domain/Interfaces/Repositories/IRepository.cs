using Domain.DTOs;
using Domain.Entities;

namespace Domain.Interfaces.Repositories;

public interface IRepository<TEntity> where TEntity : Entity
{
    Task InsertAsync(TEntity entity);

    Task UpdateAsync(TEntity entity);

    Task DeleteAsync(TEntity entity);
    
    Task<TEntity> GetByIdAsync(long id);
    
    Task<PaginatedResultDto> GetByPaginatedAsync(int pageNumber = 1, int pageSize = 25);
}