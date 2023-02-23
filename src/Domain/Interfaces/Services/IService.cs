using Domain.DTOs;
using Domain.Entities;
using FluentValidation.Results;

namespace Domain.Interfaces.Services;

public interface IService<TEntity> where TEntity : Entity
{
    Task<ValidationResult> AddAsync(TEntity entity);

    Task<ValidationResult> UpdateAsync(TEntity entity);

    Task<ValidationResult> RemoveAsync(TEntity entity);
    
    Task<TEntity> GetByIdAsync(long id);
    
    Task<PaginatedResultDto> GetByPaginatedAsync(int pageNumber = 1, int pageSize = 25);
}