using Learn.Abstractions;
using Learn.Models.NHibernate;
using Learn.Undo;
using NHibernate;

namespace Learn.NHibernate;

public class NHibernateCompanyRepository : IGenericRepository<Company>, IRepository
{
    private readonly ISession _session;

    public NHibernateCompanyRepository(ISession session)
    {
        _session = session;
    }
    public void Create(Company item)
    { 
        var id = (int)_session.Save(item);
    }
    public async Task CreateAsync(Company item)
    {
        var id = (int)(await _session.SaveAsync(item));
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
        _session.Delete(item);
    }
    public async Task RemoveAsync(Company item)
    {
        await _session.DeleteAsync(item);
    }

    public void Update(int id, Company item)
    {
        _session.Update(item);
    }
    public async Task UpdateAsync(int id, Company item)
    {
        await _session.UpdateAsync(item);
    }
}