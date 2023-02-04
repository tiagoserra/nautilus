using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;

namespace Domain.Services;

public abstract class Service<TEntity>: IService<TEntity> where TEntity : Entity
{
    private readonly IRepository<TEntity> Repository;

    protected Service(IRepository<TEntity> repository)
    {
        Repository = repository;
    }
    
    public virtual async Task<DomainValidationDto> AddAsync(TEntity entity)
    {
        if (!entity.DomainValidation.IsValid())
            return entity.DomainValidation;
            
        await Repository.InsertAsync(entity);

        return null;
    }

    public virtual async Task<DomainValidationDto> UpdateAsync(TEntity entity)
    {
        if (!entity.DomainValidation.IsValid())
            return entity.DomainValidation;
        
        await Repository.UpdateAsync(entity);
        
        return null;
    }

    public virtual async Task<DomainValidationDto> RemoveAsync(TEntity entity)
    {
        if (!entity.DomainValidation.IsValid())
            return entity.DomainValidation;
        
        await Repository.DeleteAsync(entity);
        
        return null;
    }

    public virtual async Task<TEntity> GetByIdAsync(long id)
    {
        return await Repository.GetByIdAsync(id);
    }

    public virtual async Task<PaginatedResultDto> GetByPaginatedAsync(int pageNumber = 1, int pageSize = 25)
    {
        return await Repository.GetByPaginatedAsync(pageNumber: pageNumber, pageSize: pageSize);
    }
}