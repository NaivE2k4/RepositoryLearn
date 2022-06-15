﻿using Learn.Abstractions;
using Learn.Models.Visitor;
using Learn.Undo;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Learn.EF;

//Я знаю что EF сам в себе реализует UOW и Repository
public class EFGenericRepository<TEntity> : IGenericUndoRepo<TEntity>, IUndoRepo, IRepository where TEntity : class, IVisitableModel
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
    {//If id is autogenerated, we should ask it after SaveChanges somehow (As EF puts it into entity after save)
        var visitor = new GetIdVisitor();
        item.Accept(visitor);
        _dbSet.Add(item);
        _undoCollection.Add(new UndoInfo
        {
            PrevState = item,
            OpType = UndoOpType.Create,
            Id = visitor.Result, //We dont know
            EntityType = typeof(TEntity),
        });
    }

    public async Task CreateAsync(TEntity item)
    {
        var visitor = new GetIdVisitor();
        item.Accept(visitor);
        _dbSet.Add(item);
        _undoCollection.Add(new UndoInfo
        {
            PrevState = item,
            OpType = UndoOpType.Create,
            Id = visitor.Result, //We dont know
            EntityType = typeof(TEntity),
        });
        await Task.CompletedTask;
    }

    public TEntity? FindById(int id)
    {
        return _dbSet.Find(id);
    }

    public async Task<TEntity?> FindByIdAsync(int id)
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
        var visitor = new GetIdVisitor();
        item.Accept(visitor);
        _undoCollection.Add(new UndoInfo
        {
            PrevState = item,
            OpType = UndoOpType.Delete,
            Id = visitor.Result,
            EntityType = typeof(TEntity),
        });
        _dbSet.Remove(item);
    }

    public async Task RemoveAsync(TEntity item)
    {
        var visitor = new GetIdVisitor();
        item.Accept(visitor);
        _undoCollection.Add(new UndoInfo
        {
            PrevState = item,
            OpType = UndoOpType.Delete,
            Id = visitor.Result,
            EntityType = typeof(TEntity),
        });
        _dbSet.Remove(item);
        
        await Task.CompletedTask;
    }

    public void Update(int id, TEntity item)
    {
        var entry = _dBContext.Entry(item);
        var dbValues = entry.GetDatabaseValues()?.ToObject();
        _undoCollection.Add(new UndoInfo
        {
            PrevState = dbValues ?? entry.CurrentValues.ToObject(),
            OpType = UndoOpType.Update,
            Id = id,
            EntityType = typeof(TEntity),
        });
        
        if(entry.State != EntityState.Added)
            entry.State = EntityState.Modified;
    }

    public async Task UpdateAsync(int id, TEntity item)
    {
        _undoCollection.Add(new UndoInfo
        {
            PrevState = (await _dBContext.Entry(item).GetDatabaseValuesAsync())?.ToObject(),
            OpType = UndoOpType.Update,
            Id = id,
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
                var obj = _dbSet.Find(undoInfo.Id); //Finds cached or not
                _dbSet.Remove(obj!);
                
                break;
            case UndoOpType.Update:
                var entity = _dbSet.Find(undoInfo.Id);
                var entry = _dBContext.Entry(entity!);
                entry.CurrentValues.SetValues(undoInfo.PrevState!);
                if(entry.State != EntityState.Added)
                    entry.State = EntityState.Modified;
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
