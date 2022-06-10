using Learn.Abstractions;
using Learn.NHibernate.Models;
using NHibernate;

namespace Learn.NHibernate;

public class NHibernateCompanyRepository : IGenericRepository<Company>
{
    private ISession _session;
    public NHibernateCompanyRepository(ISession session)
    {
        _session = session;
    }
    public void Create(Company item)
    { 
        _session.Save(item);
    }
    public async Task CreateAsync(Company item)
    {
        await _session.SaveAsync(item);
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
    public Task<IEnumerable<Company>> GetAsync(Func<Company, bool> predicate) => throw new NotImplementedException();
    public void Remove(Company item) => throw new NotImplementedException();
    public Task RemoveAsync(Company item) => throw new NotImplementedException();
    public void Update(Company item) => throw new NotImplementedException();
    public Task UpdateAsync(Company item) => throw new NotImplementedException();
}