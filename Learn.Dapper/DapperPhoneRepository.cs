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
public class DapperPhoneRepository : IGenericRepository<Phone>
{
    readonly IDbTransaction _dbTransaction;
    readonly IDbConnection _dbConnection;
    readonly UowUndoCollection _undoCollection;

    public DapperPhoneRepository(IDbTransaction dbTransaction, UowUndoCollection undoCollection)
    {
        _dbConnection = dbTransaction.Connection;
        _dbTransaction = dbTransaction;
        _undoCollection = undoCollection;
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
        _undoCollection.Add(item.Id, typeof(Phone), UndoOpType.Create, item);
        Execute("INSERT INTO Phones VALUES(@Id, @Name, @Price, @CompanyId)", item);
    }

    public async Task CreateAsync(Phone item)
    {
        _undoCollection.Add(item.Id, typeof(Phone), UndoOpType.Create, item);
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
        _undoCollection.Add(item.Id, typeof(Phone), UndoOpType.Delete, item);
        Execute("DELETE FROM Phones WHERE id = @ID", new { item.Id});
    }

    public async Task RemoveAsync(Phone item)
    {
        _undoCollection.Add(item.Id, typeof(Phone), UndoOpType.Delete, item);
        await ExecuteAsync("DELETE FROM Phones WHERE id = @ID", new { item.Id });
    }

    public void Update(int id, Phone item)
    {
        var existing = FindById(id);
        _undoCollection.Add(id, typeof(Phone), UndoOpType.Update, existing);
        Execute(@"UPDATE Phones
SET Name = @Name,
Price = @Price,
CompanyId = @CompanyId
WHERE id = @ID", new { item.Id });
    }

    public async Task UpdateAsync(int id, Phone item)
    {
        var existing = await FindByIdAsync(id);
        _undoCollection.Add(id, typeof(Phone), UndoOpType.Update, existing);
        await ExecuteAsync(@"UPDATE Phones
SET Name = @Name,
Price = @Price,
CompanyId = @CompanyId
WHERE id = @ID", new { item.Id });
    }

    public void UndoOperaton(UndoInfo undoInfo)
    {
        switch (undoInfo.OpType)
        {
            case UndoOpType.None:
                break;
            case UndoOpType.Create:
                Remove(undoInfo.PrevState as Phone);
                break;
            case UndoOpType.Update:
                Update(undoInfo.Id, undoInfo.PrevState as Phone);
                break;
            case UndoOpType.Delete:
                Create(undoInfo.PrevState as Phone);
                break;
        }
    }
}
