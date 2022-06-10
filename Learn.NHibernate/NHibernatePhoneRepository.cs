using Learn.Abstractions;
using Learn.NHibernate.Models;

namespace Learn.NHibernate;

public class NHibernatePhoneRepository : IGenericRepository<Phone>
{
    public void Create(Phone item) => throw new NotImplementedException();
    public Task CreateAsync(Phone item) => throw new NotImplementedException();
    public Phone FindById(int id) => throw new NotImplementedException();
    public Task<Phone> FindByIdAsync(int id) => throw new NotImplementedException();
    public IEnumerable<Phone> Get() => throw new NotImplementedException();
    public IEnumerable<Phone> Get(Func<Phone, bool> predicate) => throw new NotImplementedException();
    public Task<IEnumerable<Phone>> GetAsync() => throw new NotImplementedException();
    public Task<IEnumerable<Phone>> GetAsync(Func<Phone, bool> predicate) => throw new NotImplementedException();
    public void Remove(Phone item) => throw new NotImplementedException();
    public Task RemoveAsync(Phone item) => throw new NotImplementedException();
    public void Update(Phone item) => throw new NotImplementedException();
    public Task UpdateAsync(Phone item) => throw new NotImplementedException();
}
