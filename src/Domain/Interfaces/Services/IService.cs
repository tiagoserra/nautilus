using Domain.DTOs;
using Domain.Entities;

namespace Domain.Interfaces.Services;

public interface IService<TEntity> where TEntity : Entity
{
    Task<DomainValidationDto> AddAsync(TEntity entity);

    Task<DomainValidationDto> UpdateAsync(TEntity entity);

    Task<DomainValidationDto> RemoveAsync(TEntity entity);
    
    Task<TEntity> GetByIdAsync(long id);
    
    Task<PaginatedResultDto> GetByPaginatedAsync(int pageNumber = 1, int pageSize = 25);
}