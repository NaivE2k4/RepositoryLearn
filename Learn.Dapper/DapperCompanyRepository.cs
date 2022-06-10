using Dapper;
using Learn.Abstractions;
using RepositoryLearn.Models;
using System.Data;

namespace Learn.Dapper;
/// <summary>
/// This is a repository class to work with Unit of Work
/// More see <see cref="DapperUnitOfWork"/>
/// </summary>
public class DapperCompanyRepository : IGenericRepository<Company>
{
    IDbTransaction _dbTransaction;
    IDbConnection _dbConnection;
    
    public DapperCompanyRepository(IDbTransaction dbTransaction)
    {
        _dbConnection = dbTransaction.Connection;
        _dbTransaction = dbTransaction;
    }

    private int Execute(string sql, object param)
    { 
        //using var conn = new SQLiteConnection(_connectionString);
        return _dbConnection.Execute(sql, param, _dbTransaction);
    }

    private async Task<int> ExecuteAsync(string sql, object param)
    {
        return await _dbConnection.ExecuteAsync(sql, param, _dbTransaction);
    }
    public void Create(Company item)
    {
        Execute("INSERT INTO Companies VALUES(@id, @name)", item);
    }

    public async Task CreateAsync(Company item)
    {
        await ExecuteAsync("INSERT INTO Companies VALUES(@id, @name)", item);
    }

    public Company FindById(int id)
    {
        return _dbConnection.QueryFirstOrDefault<Company>("SELECT * FROM Companies WHERE Id = @id", new { id = id }, _dbTransaction);
    }

    public async Task<Company> FindByIdAsync(int id)
    {
        return await _dbConnection.QueryFirstOrDefaultAsync<Company>("SELECT * FROM Companies WHERE Id = @id", new { id = id }, _dbTransaction);
    }

    public IEnumerable<Company> Get()
    {
        return _dbConnection.Query<Company>("SELECT * FROM Companies", transaction: _dbTransaction);
    }

    public IEnumerable<Company> Get(Func<Company, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Company>> GetAsync()
    {
        return await _dbConnection.QueryAsync<Company>("SELECT * FROM Companies", transaction: _dbTransaction);
    }

    public Task<IEnumerable<Company>> GetAsync(Func<Company, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public void Remove(Company item)
    {
        Execute("DELETE FROM Companies WHERE id = @id", new { id = item.Id });
    }

    public async Task RemoveAsync(Company item)
    {
        await ExecuteAsync("DELETE FROM Companies WHERE id = @id", new { id = item.Id });
    }

    public void Update(Company item)
    {
        Execute("UPDATE Companies SET Name = @Name WHERE id = @id", new { id = item.Id, Name = item.Name });
    }

    public async Task UpdateAsync(Company item)
    {
        await ExecuteAsync("UPDATE Companies SET Name = @Name WHERE id = @id", new { id = item.Id, Name = item.Name });
    }
}

