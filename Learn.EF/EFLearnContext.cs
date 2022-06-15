using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite.Infrastructure.Internal;
using RepositoryLearn.Models;
using System.Data.Common;

namespace Learn.EF;

/*
 dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef migrations add InitialCreate
dotnet ef database update
 */
public class EFLearnContext : DbContext
{
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Phone> Phones => Set<Phone>();

    private readonly string _connstring;
    private readonly DbConnection _conn;

    public EFLearnContext()
        :base()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var DbPath = Path.Join(path, "blogging.db");
        _connstring = $"Data Source={DbPath}";
    }
    public EFLearnContext(DbContextOptions<EFLearnContext> options)
        :base(options)
    {
        _connstring = options.FindExtension<SqliteOptionsExtension>()?.Connection?.ConnectionString ?? "";
    }

    public EFLearnContext(string connstring)
        : base()
    {
        _connstring = connstring;
    }
    public EFLearnContext(DbConnection conn)
    : base()
    {
        _connstring = null;
        _conn = conn;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if(!options.IsConfigured)
            if(_connstring != null)
                options.UseSqlite(_connstring);
            else
                options.UseSqlite(_conn);
    }

}
