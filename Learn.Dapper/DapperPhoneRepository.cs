using Dapper;
using Learn.Abstractions;
using RepositoryLearn.Models;
using System.Data.SQLite;

namespace Learn.Dapper;


public class DapperPhoneRepository : IGenericRepository<Phone>
{
    string _connectionString;
    public DapperPhoneRepository()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var DbPath = Path.Join(path, "blogging.db");
        _connectionString = $"Data Source={DbPath}";
    }

    private int Execute(string sql, object param)
    {
        using var conn = new SQLiteConnection(_connectionString);
        return conn.Execute(sql, param);
    }

    private async Task<int> ExecuteAsync(string sql, object param)
    {
        using var conn = new SQLiteConnection(_connectionString);
        return await conn.ExecuteAsync(sql, param);
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
        using var conn = new SQLiteConnection(_connectionString);
        return conn.QueryFirstOrDefault<Phone>("SELECT Id, Name, Price, CompanyId FROM Phones WHERE Id = @Id", new { Id = id });
    }

    public async Task<Phone> FindByIdAsync(int id)
    {
        using var conn = new SQLiteConnection(_connectionString);
        return await conn.QueryFirstOrDefaultAsync<Phone>("SELECT Id, Name, Price, CompanyId FROM Phones WHERE Id = @Id", new { Id = id });
    }

    public IEnumerable<Phone> Get()
    {
        using var conn = new SQLiteConnection(_connectionString);
        return conn.Query<Phone>("SELECT Id, Name, Price, CompanyId FROM Phones");
    }

    public IEnumerable<Phone> Get(Func<Phone, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Phone>> GetAsync()
    {
        using var conn = new SQLiteConnection(_connectionString);
        return await conn.QueryAsync<Phone>("SELECT Id, Name, Price, CompanyId FROM Phones");
    }

    public Task<IEnumerable<Phone>> GetAsync(Func<Phone, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public void Remove(Phone item)
    {
        Execute("DELETE FROM Phones WHERE id = @ID", new { item.Id});
    }

    public async Task RemoveAsync(Phone item)
    {
        await ExecuteAsync("DELETE FROM Phones WHERE id = @ID", new { item.Id });
    }

    public void Update(Phone item)
    {
        Execute(@"UPDATE Phones
SET Name = @Name,
Price = @Price,
CompanyId = @CompanyId
WHERE id = @ID", new { item.Id });
    }

    public async Task UpdateAsync(Phone item)
    {
        await ExecuteAsync(@"UPDATE Phones
SET Name = @Name,
Price = @Price,
CompanyId = @CompanyId
WHERE id = @ID", new { item.Id });
    }
}
