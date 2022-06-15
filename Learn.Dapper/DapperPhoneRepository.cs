using Dapper;
using Learn.Abstractions;
using RepositoryLearn.Models;
using System.Data;

namespace Learn.Dapper;

/// <summary>
/// This is a repository class to work with Unit of Work
/// More see <see cref="DapperUnitOfWork"/>
/// </summary>
public class DapperPhoneRepository : IGenericRepository<Phone>, IRepository
{
    readonly IDbTransaction _dbTransaction;
    readonly IDbConnection _dbConnection;

    public DapperPhoneRepository(IDbTransaction dbTransaction)
    {
        _dbConnection = dbTransaction.Connection;
        _dbTransaction = dbTransaction;
    }

    private int Execute(string sql, object param)
    {
        return _dbConnection.Execute(sql, param, _dbTransaction);
    }

    private async Task<int> ExecuteAsync(string sql, object param)
    {
        return await _dbConnection.ExecuteAsync(sql, param, _dbTransaction);
    }

    public void Create(Phone item)
    {
        Execute("INSERT INTO Phones VALUES(@Id, @Name, @Price, @CompanyId)", item);
    }

    public async Task CreateAsync(Phone item)
    {
        await ExecuteAsync("INSERT INTO Phones VALUES(@Id, @Name, @Price, @CompanyId)", item);
    }

    public Phone FindById(int id)
    {
        return _dbConnection.QueryFirstOrDefault<Phone>("SELECT Id, Name, Price, CompanyId FROM Phones WHERE Id = @Id", new { Id = id }, _dbTransaction);
    }

    public async Task<Phone> FindByIdAsync(int id)
    {
        return await _dbConnection.QueryFirstOrDefaultAsync<Phone>("SELECT Id, Name, Price, CompanyId FROM Phones WHERE Id = @Id", new { Id = id }, _dbTransaction);
    }

    public IEnumerable<Phone> Get()
    {
        return _dbConnection.Query<Phone>("SELECT Id, Name, Price, CompanyId FROM Phones", transaction: _dbTransaction);
    }

    public IEnumerable<Phone> Get(Func<Phone, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Phone>> GetAsync()
    {
        return await _dbConnection.QueryAsync<Phone>("SELECT Id, Name, Price, CompanyId FROM Phones", transaction: _dbTransaction);
    }

    public Task<IEnumerable<Phone>> GetAsync(Func<Phone, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public void Remove(Phone item)
    {
        Execute("DELETE FROM Phones WHERE id = @ID", new { item.Id });
    }

    public async Task RemoveAsync(Phone item)
    {
        await ExecuteAsync("DELETE FROM Phones WHERE id = @ID", new { item.Id });
    }

    public void Update(int id, Phone item)
    {
        Execute(@"UPDATE Phones
SET Name = @Name,
Price = @Price,
CompanyId = @CompanyId
WHERE id = @ID", new { item.Id });
    }

    public async Task UpdateAsync(int id, Phone item)
    {
        await ExecuteAsync(@"UPDATE Phones
SET Name = @Name,
Price = @Price,
CompanyId = @CompanyId
WHERE id = @ID", new { item.Id });
    }
}
