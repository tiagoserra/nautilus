using Domain.DTOs;
using Domain.Entites;
using FluentValidation.Results;

namespace Domain.Interfaces.Services;

public interface IService<TEntity> where TEntity : Entity<TEntity>
{
    ValidationResult Add(TEntity entity);

    ValidationResult Update(TEntity entity);

    ValidationResult Remove(TEntity entity);
    
    TEntity GetById(long id);
    
    PaginatedResultDto GetByPaginated(int pageNumber = 1, int pageSize = 25);
}