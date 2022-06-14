using Learn.EF;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RepositoryLearn.Models;

namespace Learn.Tests;

/// <summary>
/// THIS IS A BAD TEST DESIGN DO NOT DO THIS!
/// 
/// </summary>
public class EFUowTests : IDisposable
{
    EFUnitOfWork<EFLearnContext> _uow;
    readonly EFLearnContext context;
    public EFUowTests()
    {
        //I dont know why, but this works and UseSqlite in OnConfiguring - not
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();
        var option = new DbContextOptionsBuilder<EFLearnContext>().UseSqlite(connection).Options;
        context = new EFLearnContext(option);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        _uow = new EFUnitOfWork<EFLearnContext>(context);
        context.Companies.Add(new Company { Id = 1, Name = "First" });
        context.Companies.Add(new Company { Id = 2, Name = "Second" });
        context.Phones.Add(new Phone { Id = 1, Name = "FirstPhone", CompanyId = 1, Price = 100 });
        context.Phones.Add(new Phone { Id = 2, Name = "SecondPhone", CompanyId = 2, Price = 300 });
        context.SaveChanges();
        //context.Database.Migrate();
    }
    
    [Fact]
    public async Task DatabaseIsAvailableAndCanBeConnectedTo()
    {
        Assert.True(await context.Database.CanConnectAsync());
    }

    [Fact]
    public void TestGetById()
    {
        var company = _uow.Companies.FindById(1);
        Assert.NotNull(company);
        Assert.True(company.Name == "First");

        var phone = _uow.Phones.FindById(2);
        Assert.NotNull(phone);
        Assert.True(phone.Name == "SecondPhone");

        var nocompany = _uow.Companies.FindById(900);
        Assert.Null(nocompany);
    }

    [Fact]
    public void TestCreateAndUndoCreate()
    {
        var company3 = new Company { Id = 3, Name = "Third" };
        _uow.Companies.Create(company3);
        _uow.Save();
        _uow.Start();
        var check = _uow.Companies.FindById(3);
        Assert.NotNull(check);
        Assert.True(check.Name == company3.Name);

        _uow.Undo();
        _uow.Save();

        _uow.Start();
        var check2 = _uow.Companies.FindById(3);
        Assert.Null(check2);
    }

    public void Dispose()
    {
        _uow?.Dispose();
        context?.Dispose();
    }
}