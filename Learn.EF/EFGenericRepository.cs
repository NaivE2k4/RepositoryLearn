using Learn.Abstractions;
using Learn.Undo;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Learn.EF;

//Я знаю что EF сам в себе реализует UOW и Repository
public class EFGenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    readonly DbContext _dBContext;
    readonly DbSet<TEntity> _dbSet;
    readonly UowUndoCollection _undoCollection;
    public EFGenericRepository(DbContext dbContext, UowUndoCollection undoCollection)
    {
        _dBContext = dbContext;
        _dbSet = _dBContext.Set<TEntity>();
        _undoCollection = undoCollection;
    }

    public void Create(TEntity item)
    {
        _dbSet.Add(item);
        _undoCollection.Add(new UndoInfo
        {
            PrevState = item,
            OpType = UndoOpType.Create,
            Id = -1, //We dont know
            EntityType = typeof(TEntity),
        });
    }

    public async Task CreateAsync(TEntity item)
    {
        _dbSet.Add(item);
        _undoCollection.Add(new UndoInfo
        {
            PrevState = item,
            OpType = UndoOpType.Create,
            Id = -1, //We dont know
            EntityType = typeof(TEntity),
        });
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
        _undoCollection.Add(new UndoInfo
        {
            PrevState = item,
            OpType = UndoOpType.Delete,
            Id = -1,
            EntityType = typeof(TEntity),
        });
        _dbSet.Remove(item);
    }

    public async Task RemoveAsync(TEntity item)
    {
        _dbSet.Remove(item);
        _undoCollection.Add(new UndoInfo
        {
            PrevState = item,
            OpType = UndoOpType.Delete,
            Id = -1,
            EntityType = typeof(TEntity),
        });
        await Task.CompletedTask;
    }

    public void Update(TEntity item)
    {
        _undoCollection.Add(new UndoInfo
        {
            PrevState = item,
            OpType = UndoOpType.Update,
            Id = -1,
            EntityType = typeof(TEntity),
        });
        _dBContext.Entry(item).State = EntityState.Modified;
    }

    public async Task UpdateAsync(TEntity item)
    {
        _undoCollection.Add(new UndoInfo
        {
            PrevState = item,
            OpType = UndoOpType.Update,
            Id = -1,
            EntityType = typeof(TEntity),
        });
        _dBContext.Entry(item).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    //single easy undo
    public void UndoOperaton(UndoInfo undoInfo)
    {
        switch(undoInfo.OpType)
        {
            case UndoOpType.None:
                break;
            case UndoOpType.Create:
                _dbSet.Remove((TEntity)undoInfo.PrevState!);
                break;
            case UndoOpType.Update:
                _dBContext.Entry(undoInfo.PrevState!).State = EntityState.Modified;
                break;
            case UndoOpType.Delete:
                _dbSet.Add((TEntity)undoInfo.PrevState!);
                break;
        }
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
