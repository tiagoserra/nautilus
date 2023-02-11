using Domain.DTOs;
using Domain.Entities;

namespace Domain.Interfaces.Services;

public interface IService<TEntity> where TEntity : Entity
{
    Task<DomainValidation> AddAsync(TEntity entity);

    Task<DomainValidation> UpdateAsync(TEntity entity);

    Task<DomainValidation> RemoveAsync(TEntity entity);
    
    Task<TEntity> GetByIdAsync(long id);
    
    Task<PaginatedResultDto> GetByPaginatedAsync(int pageNumber = 1, int pageSize = 25);
}