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
        Assert.True(company!.Name == "First");

        var phone = _uow.Phones.FindById(2);
        Assert.NotNull(phone);
        Assert.True(phone!.Name == "SecondPhone");

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
        Assert.True(check!.Name == company3.Name);

        _uow.Undo(); //Undo saves

        _uow.Start();
        var check2 = _uow.Companies.FindById(3);
        Assert.Null(check2);
        var check3 = _uow.Companies.FindById(2);
        Assert.Null(check2);
    }

    [Fact]
    public void TestUpdateAndUndoUpdate()
    {
        var targetCompanyId = 2;
        var checkName = "Blabla";

        var company2 = _uow.Companies.FindById(targetCompanyId);
        company2!.Name = checkName;
        
        _uow.Companies.Update(targetCompanyId, company2);
        _uow.Save();
        _uow.Start();

        var check = _uow.Companies.FindById(targetCompanyId);
        Assert.True(check!.Name == checkName);

        _uow.Undo();

        _uow.Start();
        var check2 = _uow.Companies.FindById(targetCompanyId);
        Assert.True(check.Name == "Second");
    }

    [Fact]
    public void TestRemoveAndUndoRemove()
    {
        var targetCompanyId = 2;
        var company2 = _uow.Companies.FindById(targetCompanyId);
        _uow.Companies.Remove(company2!);
        _uow.Save();

        _uow.Start();
        var check = _uow.Companies.FindById(targetCompanyId);
        Assert.Null(check);

        _uow.Undo();
        _uow.Start();
        var check2 = _uow.Companies.FindById(targetCompanyId);
        Assert.NotNull(check2);
    }

    [Fact]
    public void TestMakeAndUndoSeveralChanges()
    {
        var company3 = new Company { Id = 3, Name = "Third" };
        _uow.Companies.Create(company3);
        company3.Name = "BlaBla";
        _uow.Companies.Update(3, company3);
        _uow.Companies.Remove(company3);
        _uow.Save();
        
        var check = _uow.Companies.FindById(3);
        Assert.Null(check);

        _uow.Undo();
        var check2 = _uow.Companies.FindById(3);
        Assert.Null(check);
    }

    public void Dispose()
    {
        _uow?.Dispose();
        context?.Dispose();
    }
}