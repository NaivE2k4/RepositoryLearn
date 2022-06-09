using Dapper;
using Learn.Abstractions;
using RepositoryLearn.Models;
using System.Data.SQLite;

namespace Learn.Dapper;
public class DapperCompanyRepository : IGenericRepository<Company>
{
    string _connectionString;
    public DapperCompanyRepository()
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
        using var conn = new SQLiteConnection(_connectionString);
        return conn.QueryFirstOrDefault<Company>("SELECT * FROM Companies WHERE Id = @id", new { id = id });
    }

    public async Task<Company> FindByIdAsync(int id)
    {
        using var conn = new SQLiteConnection(_connectionString);
        return await conn.QueryFirstOrDefaultAsync<Company>("SELECT * FROM Companies WHERE Id = @id", new { id = id });
    }

    public IEnumerable<Company> Get()
    {
        using var conn = new SQLiteConnection(_connectionString);
        return conn.Query<Company>("SELECT * FROM Companies");
    }

    public IEnumerable<Company> Get(Func<Company, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Company>> GetAsync()
    {
        using var conn = new SQLiteConnection(_connectionString);
        return await conn.QueryAsync<Company>("SELECT * FROM Companies");
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

