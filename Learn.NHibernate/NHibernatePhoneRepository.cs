using Learn.Abstractions;
using Learn.Models.NHibernate;
using NHibernate;

namespace Learn.NHibernate;

public class NHibernatePhoneRepository : IGenericRepository<Phone>, IRepository
{
    private readonly ISession _session;
    public NHibernatePhoneRepository(ISession session)
    {
        _session = session;
    }
    public void Create(Phone item)
    {
        var id = (int) _session.Save(item);
    }
    public async Task CreateAsync(Phone item)
    {
        var id = (int) await _session.SaveAsync(item);
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

    public void Update(int id, Phone item)
    {
        _session.Update(item);
    }
    public async Task UpdateAsync(int id, Phone item)
    {
        await _session.UpdateAsync(item);
    }
}
