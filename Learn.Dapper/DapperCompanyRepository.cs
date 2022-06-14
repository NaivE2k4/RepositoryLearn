using Dapper;
using Learn.Abstractions;
using Learn.Undo;
using RepositoryLearn.Models;
using System.Data;

namespace Learn.Dapper;
/// <summary>
/// This is a repository class to work with Unit of Work
/// More see <see cref="DapperUnitOfWork"/>
/// </summary>
public class DapperCompanyRepository : IGenericRepository<Company>
{
    readonly IDbTransaction _dbTransaction;
    readonly IDbConnection _dbConnection;
    readonly UowUndoCollection _undoCollection;

    public DapperCompanyRepository(IDbTransaction dbTransaction, UowUndoCollection undoCollection)
    {
        _dbConnection = dbTransaction.Connection;
        _dbTransaction = dbTransaction;
        _undoCollection = undoCollection;
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
        _undoCollection.Add(item.Id, typeof(Company), UndoOpType.Create, item);
        Execute("INSERT INTO Companies VALUES(@id, @name)", item);
    }

    public async Task CreateAsync(Company item)
    {
        _undoCollection.Add(item.Id, typeof(Company), UndoOpType.Create, item);
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
        _undoCollection.Add(item.Id, typeof(Company), UndoOpType.Delete, item);
        Execute("DELETE FROM Companies WHERE id = @id", new { id = item.Id });
    }

    public async Task RemoveAsync(Company item)
    {
        _undoCollection.Add(item.Id, typeof(Company), UndoOpType.Delete, item);
        await ExecuteAsync("DELETE FROM Companies WHERE id = @id", new { id = item.Id });
    }

    public void Update(int id, Company item)
    {
        var existing = FindById(id);
        _undoCollection.Add(id, typeof(Company), UndoOpType.Update, existing);
        Execute("UPDATE Companies SET Name = @Name WHERE id = @id", new { id = item.Id, Name = item.Name });
    }

    public async Task UpdateAsync(int id, Company item)
    {
        var existing = await FindByIdAsync(id);
        _undoCollection.Add(id, typeof(Company), UndoOpType.Update, existing);
        await ExecuteAsync("UPDATE Companies SET Name = @Name WHERE id = @id", new { id = item.Id, Name = item.Name });
    }

    public void UndoOperaton(UndoInfo undoInfo)
    {
        switch(undoInfo.OpType)
        {
            case UndoOpType.None:
                break;
            case UndoOpType.Create:
                Remove(undoInfo.PrevState as Company);
                break;
            case UndoOpType.Update:
                Update(undoInfo.Id, undoInfo.PrevState as Company);
                break;
            case UndoOpType.Delete:
                Create(undoInfo.PrevState as Company);
                break;
        }
    }
}

