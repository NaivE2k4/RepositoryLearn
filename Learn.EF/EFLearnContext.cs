using Microsoft.EntityFrameworkCore;
using RepositoryLearn.Models;

namespace Learn.EF;

/*
 dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef migrations add InitialCreate
dotnet ef database update
 */
public class EFLearnContext : DbContext
{
    public DbSet<Company> Companies { get; set; }
    public DbSet<Phone> Phones { get; set; }

    private readonly string _connstring;

    public EFLearnContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var DbPath = Path.Join(path, "blogging.db");
        _connstring = $"Data Source={DbPath}";
    }

    public EFLearnContext(string connstring)
    {
        _connstring = connstring;
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite(_connstring);
}
