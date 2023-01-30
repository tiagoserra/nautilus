using Domain.DTOs;
using Domain.Entites;

namespace Domain.Interfaces.Repositories;

public interface IRepository<TEntity> where TEntity : Entity<TEntity>
{
    void Insert(TEntity entity);

    void Update(TEntity entity);

    void Delete(TEntity entity);
    
    TEntity GetById(long id);
    
    PaginatedResultDto GetByPaginated(int pageNumber = 1, int pageSize = 25);
}