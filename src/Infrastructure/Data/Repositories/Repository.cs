using Domain.DTOs;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Infrastructure.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using SqlKata.Execution;

namespace Infrastructure.Data.Repositories;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : Entity
{
    protected SqlServerContext Context;
    
    protected DbSet<TEntity> Entity;
    
    protected readonly QueryFactory QueryFactory;

    public Repository(SqlServerContext context, QueryFactory queryFactory)
    {
        Context = context;
        QueryFactory = queryFactory;
        Entity = context.Set<TEntity>();
    }

    public async Task InsertAsync(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException("entity");

        Entity.Add(entity);

        await Context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException("entity");

        Entity.Update(entity);

        await Context.SaveChangesAsync();
    }

    public async Task DeleteAsync(TEntity entity)
    {
        if (entity == null)
            throw new ArgumentNullException("entity");

        Entity.Remove(entity);

        await Context.SaveChangesAsync();
    }

    public async Task<TEntity> GetByIdAsync(long id)
        => await QueryFactory.Query(typeof(TEntity).Name).Where("Id", id).FirstAsync<TEntity>();

    public async Task<PaginatedResultDto> GetByPaginatedAsync(int pageNumber = 1, int pageSize = 25)
    {
        var query = QueryFactory.Query(typeof(TEntity).Name);
        var data = await query.OrderBy("Id").PaginateAsync(pageNumber, pageSize);

        return data is null ? new PaginatedResultDto(0,0,0,0, null) : new PaginatedResultDto(data.TotalPages, pageNumber, pageSize, data.Count, data.List);
    }
}