using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces.Repositories;

namespace Domain.Services;

public abstract class Service<TEntity> where TEntity : Entity
{
    private readonly IRepository<TEntity> Repository;

    protected Service(IRepository<TEntity> repository)
        => Repository = repository;

    public virtual async Task AddAsync(TEntity entity)
        => await Repository.InsertAsync(entity);

    public virtual async Task UpdateAsync(TEntity entity)
        => await Repository.UpdateAsync(entity);

    public virtual async Task RemoveAsync(TEntity entity)
        => await Repository.DeleteAsync(entity);

    public virtual async Task<TEntity> GetByIdAsync(long id)
        => await Repository.GetByIdAsync(id);

    public virtual async Task<PaginatedResultDto> GetByPaginatedAsync(int pageNumber = 1, int pageSize = 25)
        => await Repository.GetByPaginatedAsync(pageNumber: pageNumber, pageSize: pageSize);
}