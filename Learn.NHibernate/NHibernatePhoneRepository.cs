using Learn.Abstractions;
using Learn.NHibernate.Models;
using Learn.Undo;
using NHibernate;

namespace Learn.NHibernate;

public class NHibernatePhoneRepository : IGenericRepository<Phone>
{
    private readonly ISession _session;
    public NHibernatePhoneRepository(ISession session)
    {
        _session = session;
    }
    public void Create(Phone item)
    {
        _session.Save(item);
    }
    public async Task CreateAsync(Phone item)
    {
        await _session.SaveAsync(item);
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
        _session.Delete(item);
    }
    public async Task RemoveAsync(Phone item)
    {
        await _session.DeleteAsync(item);
    }

    public void UndoOperaton(UndoInfo undoInfo)
    {
        throw new NotImplementedException();
    }

    public void Update(Phone item)
    {
        _session.Update(item);
    }
    public async Task UpdateAsync(Phone item)
    {
        await _session.UpdateAsync(item);
    }
}
