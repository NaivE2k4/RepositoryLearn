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
    public void Create(Company item) => throw new NotImplementedException();
    public Task CreateAsync(Company item) => throw new NotImplementedException();
    public Company FindById(int id) => throw new NotImplementedException();
    public Task<Company> FindByIdAsync(int id) => throw new NotImplementedException();
    public IEnumerable<Company> Get() => throw new NotImplementedException();
    public IEnumerable<Company> Get(Func<Company, bool> predicate) => throw new NotImplementedException();
    public Task<IEnumerable<Company>> GetAsync() => throw new NotImplementedException();
    public Task<IEnumerable<Company>> GetAsync(Func<Company, bool> predicate) => throw new NotImplementedException();
    public void Remove(Company item) => throw new NotImplementedException();
    public Task RemoveAsync(Company item) => throw new NotImplementedException();
    public void Update(Company item) => throw new NotImplementedException();
    public Task UpdateAsync(Company item) => throw new NotImplementedException();
}