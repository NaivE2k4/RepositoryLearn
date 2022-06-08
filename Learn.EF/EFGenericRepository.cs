using Learn.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Learn.EF;

//Я знаю что EF сам в себе реализует UOW и Repository
public class EFGenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    readonly DbContext _dBContext;
    readonly DbSet<TEntity> _dbSet;
    public EFGenericRepository(DbContext dbContext)
    {
        _dBContext = dbContext;
        _dbSet = _dBContext.Set<TEntity>();
    }

    public void Create(TEntity item)
    {
        _dbSet.Add(item);
    }

    public async Task CreateAsync(TEntity item)
    {
        _dbSet.Add(item);
        await Task.CompletedTask;
    }

    public TEntity FindById(int id)
    {
        return _dbSet.Find(id);
    }

    public async Task<TEntity> FindByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public IEnumerable<TEntity> Get()
    {
        return _dbSet.AsNoTracking().ToList();
    }

    public async Task<IEnumerable<TEntity>> GetAsync()
    {
        var list = await _dbSet.AsNoTracking().ToListAsync();
        return list.AsEnumerable();
    }

    public IEnumerable<TEntity> Get(Func<TEntity, bool> predicate)
    {
        return _dbSet.AsNoTracking().Where(predicate).ToList();
    }

    public async Task<IEnumerable<TEntity>> GetAsync(Func<TEntity, bool> predicate)
    {
        var list = await _dbSet.AsNoTracking().Where(predicate).AsQueryable().ToListAsync();
        return list.AsEnumerable();
    }

    public void Remove(TEntity item)
    {
        _dbSet.Remove(item);
    }

    public async Task RemoveAsync(TEntity item)
    {
        _dbSet.Remove(item);
        await Task.CompletedTask;
    }

    public void Update(TEntity item)
    {
        _dBContext.Entry(item).State = EntityState.Modified;
    }

    public async Task UpdateAsync(TEntity item)
    {
        _dBContext.Entry(item).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public IEnumerable<TEntity> GetWithInclude(params Expression<Func<TEntity, object>>[] includeProperties)
    {
        return Include(includeProperties).ToList();
    }

    public async Task<IEnumerable<TEntity>> GetWithIncludeAsync(params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var list = await Include(includeProperties).ToListAsync();
        return list.AsEnumerable();
    }

    public IEnumerable<TEntity> GetWithInclude(Func<TEntity, bool> predicate,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query = Include(includeProperties);
        return query.Where(predicate).ToList();
    }

    public async Task<IEnumerable<TEntity>> GetWithIncludeAsync(Func<TEntity, bool> predicate,
params Expression<Func<TEntity, object>>[] includeProperties)
    {
        var query = await IncludeAsync(includeProperties);
        var list = await query.Where(predicate).AsQueryable().ToListAsync();
        return list.AsEnumerable();
    }

    private IQueryable<TEntity> Include(params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> query = _dbSet.AsNoTracking();
        return includeProperties
            .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
    }

    private async Task<IQueryable<TEntity>> IncludeAsync(params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> query = _dbSet.AsNoTracking();
        return await Task.FromResult(includeProperties
            .Aggregate(query, (current, includeProperty) => current.Include(includeProperty)));
    }

}
