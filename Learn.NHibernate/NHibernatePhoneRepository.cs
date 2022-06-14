using Learn.Abstractions;
using Learn.NHibernate.Models;
using Learn.Undo;
using NHibernate;

namespace Learn.NHibernate;

public class NHibernatePhoneRepository : IGenericRepository<Phone>
{
    private readonly ISession _session;
    private readonly UowUndoCollection _undoCollection;
    public NHibernatePhoneRepository(ISession session, UowUndoCollection undoCollection)
    {
        _session = session;
        _undoCollection = undoCollection;
    }
    public void Create(Phone item)
    {
        var id = (int)_session.Save(item);
        _undoCollection.Add(new UndoInfo
        {
            Id = id,
            EntityType = typeof(Phone),
            OpType = UndoOpType.Create,
            PrevState = item
        });
    }
    public async Task CreateAsync(Phone item)
    {
        var id = (int)await _session.SaveAsync(item);
        _undoCollection.Add(new UndoInfo
        {
            Id = id,
            EntityType = typeof(Phone),
            OpType = UndoOpType.Create,
            PrevState = item
        });
    }
    public Phone FindById(int id)
    {
        return _session.Get<Phone>(id);
    }
    public async Task<Phone> FindByIdAsync(int id)
    {
        return await _session.GetAsync<Phone>(id);
    }

    public IEnumerable<Phone> Get()
    {
        return _session.Query<Phone>().ToList();
    }

    public IEnumerable<Phone> Get(Func<Phone, bool> predicate)
    {
        return _session.Query<Phone>().Where(predicate).ToList();
    }
    public async Task<IEnumerable<Phone>> GetAsync()
    {
        var result = _session.Query<Phone>().ToList();
        return await Task.FromResult(result);
    }
    public async Task<IEnumerable<Phone>> GetAsync(Func<Phone, bool> predicate)
    {
        var result = _session.Query<Phone>().Where(predicate).ToList();
        return await Task.FromResult(result);
    }

    public void Remove(Phone item)
    {
        _undoCollection.Add(new UndoInfo
        {
            Id = item.Id,
            EntityType = typeof(Phone),
            OpType = UndoOpType.Delete,
            PrevState = item
        });
        _session.Delete(item);
    }
    public async Task RemoveAsync(Phone item)
    {
        _undoCollection.Add(new UndoInfo
        {
            Id = item.Id,
            EntityType = typeof(Phone),
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
                Remove((Phone) undoInfo.PrevState!);
                break;
            case UndoOpType.Update:
                Update(undoInfo.Id, (Phone) undoInfo.PrevState!);
                break;
            case UndoOpType.Delete:
                Create((Phone) undoInfo.PrevState!);
                break;
        }
    }

    public void Update(int id, Phone item)
    {
        var existing = FindById(id);
        _undoCollection.Add(new UndoInfo
        {
            Id = id,
            EntityType = typeof(Phone),
            OpType = UndoOpType.Update,
            PrevState = existing
        });
        _session.Update(item);
    }
    public async Task UpdateAsync(int id, Phone item)
    {
        var existing = await FindByIdAsync(id);
        _undoCollection.Add(new UndoInfo
        {
            Id = id,
            EntityType = typeof(Phone),
            OpType = UndoOpType.Update,
            PrevState = existing
        });
        await _session.UpdateAsync(item);
    }
}
