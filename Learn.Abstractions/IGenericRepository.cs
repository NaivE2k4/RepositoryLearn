namespace Learn.Abstractions;
public interface IGenericRepository<TEntity>: IRepository where TEntity : class
{
    void Create(TEntity item);
    Task CreateAsync(TEntity item);
    TEntity FindById(int id);
    Task<TEntity> FindByIdAsync(int id);
    IEnumerable<TEntity> Get();
    Task<IEnumerable<TEntity>> GetAsync();
    IEnumerable<TEntity> Get(Func<TEntity, bool> predicate);
    Task<IEnumerable<TEntity>> GetAsync(Func<TEntity, bool> predicate);
    void Remove(TEntity item);
    Task RemoveAsync(TEntity item);
    //Id is needed for Undo
    void Update(int id,TEntity item);
    Task UpdateAsync(int id, TEntity item);

}

