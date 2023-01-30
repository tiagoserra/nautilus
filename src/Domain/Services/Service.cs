using Domain.DTOs;
using Domain.Entites;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services;
using FluentValidation.Results;

namespace Domain.Services;

public abstract class Service<TEntity>: IService<TEntity> where TEntity : Entity<TEntity>
{
    protected readonly IRepository<TEntity> Repository;

    protected Service(IRepository<TEntity> repository)
    {
        Repository = repository;
    }
    
    public virtual ValidationResult Add(TEntity entity)
    {
        if (!entity.IsValid())
            return entity.ValidationResult;
            
        Repository.Insert(entity);

        return null;
    }

    public virtual ValidationResult Update(TEntity entity)
    {
        if (!entity.IsValid())
            return entity.ValidationResult;
        
        Repository.Update(entity);
        
        return null;
    }

    public virtual ValidationResult Remove(TEntity entity)
    {
        if (!entity.IsValid())
            return entity.ValidationResult;
        
        Repository.Delete(entity);
        
        return null;
    }

    public virtual TEntity GetById(long id)
    {
        return Repository.GetById(id);
    }

    public virtual PaginatedResultDto GetByPaginated(int pageNumber = 1, int pageSize = 25)
    {
        return Repository.GetByPaginated(pageNumber: pageNumber, pageSize: pageSize);
    }
}