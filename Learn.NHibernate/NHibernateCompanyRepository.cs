using Learn.Abstractions;
using Learn.NHibernate.Models;
using Learn.Undo;
using NHibernate;

namespace Learn.NHibernate;

public class NHibernateCompanyRepository : IGenericRepository<Company>
{
    private readonly ISession _session;
    private readonly UowUndoCollection _undoCollection;

    public NHibernateCompanyRepository(ISession session, UowUndoCollection undoCollection)
    {
        _session = session;
        _undoCollection = undoCollection;
    }
    public void Create(Company item)
    { 
        var id = (int)_session.Save(item);
        _undoCollection.Add(new UndoInfo
        {
            Id = id,
            EntityType = typeof(Company),
            OpType = UndoOpType.Create,
            PrevState = item
        });
    }
    public async Task CreateAsync(Company item)
    {
        var id = (int)(await _session.SaveAsync(item));
        _undoCollection.Add(new UndoInfo
        {
            Id = id,
            EntityType = typeof(Company),
            OpType = UndoOpType.Create,
            PrevState = item
        });
    }
    public Company FindById(int id)
    {
        return _session.Get<Company>(id);
    }
    public async Task<Company> FindByIdAsync(int id)
    {
        return await _session.GetAsync<Company>(id);
    }

    public IEnumerable<Company> Get()
    {
        return _session.Query<Company>().ToList();
    }

    public IEnumerable<Company> Get(Func<Company, bool> predicate)
    { 
        return _session.Query<Company>().Where(predicate).ToList();
    }
    public async Task<IEnumerable<Company>> GetAsync()
    {
        var result =_session.Query<Company>().ToList();
        return await Task.FromResult(result);
    }
    public async Task<IEnumerable<Company>> GetAsync(Func<Company, bool> predicate)
    {
        var result = _session.Query<Company>().Where(predicate).ToList();
        return await Task.FromResult(result);
    }

    public void Remove(Company item)
    {
        _undoCollection.Add(new UndoInfo
        {
            Id = item.Id,
            EntityType = typeof(Company),
            OpType = UndoOpType.Delete,
            PrevState = item
        });
        _session.Delete(item);
    }
    public async Task RemoveAsync(Company item)
    {
        _undoCollection.Add(new UndoInfo
        {
            Id = item.Id,
            EntityType = typeof(Company),
            OpType = UndoOpType.Delete,
            PrevState = item
        });
        await _session.DeleteAsync(item);
    }

    public void UndoOperaton(UndoInfo undoInfo)
    {
        switch(undoInfo.OpType)
        {
            case UndoOpType.None:
                break;
            case UndoOpType.Create:
                Remove((Company) undoInfo.PrevState!);
                break;
            case UndoOpType.Update:
                Update(undoInfo.Id, (Company) undoInfo.PrevState!);
                break;
            case UndoOpType.Delete:
                Create((Company) undoInfo.PrevState!);
                break;
        }
    }

    public void Update(int id, Company item)
    {
        var existing = FindById(id);
        _undoCollection.Add(new UndoInfo
        {
            Id = id,
            EntityType = typeof(Company),
            OpType = UndoOpType.Update,
            PrevState = existing
        });
        _session.Update(item);
    }
    public async Task UpdateAsync(int id, Company item)
    {
        var existing = await FindByIdAsync(id);
        _undoCollection.Add(new UndoInfo
        {
            Id = id,
            EntityType = typeof(Company),
            OpType = UndoOpType.Update,
            PrevState = existing
        });
        await _session.UpdateAsync(item);
    }
}