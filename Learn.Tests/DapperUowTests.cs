using Learn.Dapper;
using Learn.EF;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RepositoryLearn.Models;


namespace Learn.Tests
{
    public class DapperUowTests : IDisposable
    {
        private DapperUnitOfWork _uow;
        //private EFLearnContext context;
        public DapperUowTests()
        {
            //InitDb with EF context
            using var connection = new SqliteConnection("DataSource=:memory:;mode=memory;cache=shared");
            connection.Open();
            var option = new DbContextOptionsBuilder<EFLearnContext>().UseSqlite(connection).Options;
            using var context = new EFLearnContext(option);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.Companies.Add(new Company { Id = 1, Name = "First" });
            context.Companies.Add(new Company { Id = 2, Name = "Second" });
            context.Phones.Add(new Phone { Id = 1, Name = "FirstPhone", CompanyId = 1, Price = 100 });
            context.Phones.Add(new Phone { Id = 2, Name = "SecondPhone", CompanyId = 2, Price = 300 });
            context.SaveChanges();
            //context.Dispose();
            _uow = new DapperUnitOfWork("DataSource=:memory:;mode=memory;cache=shared");
        }

        [Fact]
        public void TestGetById()
        {
            //using var _uow = new DapperUnitOfWork("DataSource=:memory:;mode=memory;cache=shared"); 
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
            //using var _uow = new DapperUnitOfWork("DataSource=:memory:;mode=memory;cache=shared");

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
        }

        [Fact]
        public void TestUpdateAndUndoUpdate()
        {
            //using var _uow = new DapperUnitOfWork("DataSource=:memory:;mode=memory;cache=shared");

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
            //using var _uow = new DapperUnitOfWork("DataSource=:memory:;mode=memory;cache=shared");

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
            //using var _uow = new DapperUnitOfWork("DataSource=:memory:;mode=memory;cache=shared");

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
            //context?.Dispose();
            _uow?.Dispose();
        }
    }
}
