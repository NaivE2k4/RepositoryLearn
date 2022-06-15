using Learn.Abstractions;
using Learn.Models.Visitor;

namespace Learn.Undo;

/// <summary>
/// A wrapper class for dapper repositories to support undo
/// problem: if a repository is responsible for tracking undo info (for instance on create), it is hard to do actual undo
/// without adding new undo info (delete created obj without adding info to undoCollection). Well, not SO hard, but you should ether
/// add NoUndo flag to repo interface (not good, this details should be hidden from class user) or integrate this flag into repository (tracking logic, not good)
/// or duplicate obj manipulation code into Undo method (e.g. not call Repo's Delete but deleteing in Undo) - no good either
/// solution: SRP! Make a wrapper class to track undo info and bypass actual obj manipulation to repo
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class GenericUndoRepository<TEntity> : IGenericUndoRepo<TEntity>, IUndoRepo where TEntity : class, IVisitableModel, ICloneable
{
    private readonly IGenericRepository<TEntity> _repo;
    private readonly UowUndoCollection _undoCollection;

    public GenericUndoRepository(IGenericRepository<TEntity> repo, UowUndoCollection undoCollection)
    {
        _repo = repo;
        _undoCollection = undoCollection;
    }

    private static int GetId(TEntity item)
    {
        var visitor = new GetIdVisitor();
        item.Accept(visitor);
        return visitor.Result;
    }

    public void Create(TEntity item)
    {
        _undoCollection.Add(GetId(item), typeof(TEntity), UndoOpType.Create, item);
        _repo.Create(item);
    }

    public async Task CreateAsync(TEntity item)
    {
        _undoCollection.Add(GetId(item), typeof(TEntity), UndoOpType.Create, item);
        await _repo.CreateAsync(item);
    }

    public TEntity? FindById(int id) => _repo.FindById(id);

    public async Task<TEntity?> FindByIdAsync(int id) => await _repo.FindByIdAsync(id);

    public IEnumerable<TEntity> Get() => _repo.Get();

    public IEnumerable<TEntity> Get(Func<TEntity, bool> predicate) => _repo.Get(predicate);

    public async Task<IEnumerable<TEntity>> GetAsync() => await _repo.GetAsync();

    public async Task<IEnumerable<TEntity>> GetAsync(Func<TEntity, bool> predicate) => await _repo.GetAsync(predicate);

    public void Remove(TEntity item)
    {
        _undoCollection.Add(GetId(item), typeof(TEntity), UndoOpType.Delete, item);
        _repo.Remove(item);
    }

    public async Task RemoveAsync(TEntity item)
    {
        _undoCollection.Add(GetId(item), typeof(TEntity), UndoOpType.Delete, item);
        await _repo.RemoveAsync(item);
    }

    public void UndoOperaton(UndoInfo undoInfo)
    {
        switch(undoInfo.OpType)
        {
            case UndoOpType.None:
                break;
            case UndoOpType.Create:
                _repo.Remove((TEntity) undoInfo.PrevState!);
                break;
            case UndoOpType.Update:
                _repo.Update(undoInfo.Id, (TEntity) undoInfo.PrevState!);
                break;
            case UndoOpType.Delete:
                _repo.Create((TEntity) undoInfo.PrevState!);
                break;
        }
    }

    public void Update(int id, TEntity item)
    {
        var existing = _repo.FindById(id);
        var exCopy = (TEntity) existing.Clone();
        _undoCollection.Add(id, typeof(TEntity), UndoOpType.Update, exCopy);
        _repo.Update(id, item);
    }

    public async Task UpdateAsync(int id, TEntity item)
    {
        var existing = _repo.FindById(id);
        _undoCollection.Add(id, typeof(TEntity), UndoOpType.Update, existing);
        await _repo.UpdateAsync(id, item);
    }
}
