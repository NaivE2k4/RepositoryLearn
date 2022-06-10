///
///Это просто личный проект для практики разных базовых штук по работе с данными
///В нем нет чистоты кода или технического совершенства - просто отработки концепций и идей
///Ссылки:
///
///https://www.youtube.com/watch?v=3yPpL1rEK9o
///https://www.youtube.com/watch?v=oP_OUiIK4Rc
///https://stackoverflow.com/questions/42957140/repository-design-pattern-with-dapper
///https://dejanstojanovic.net/aspnet/2021/november/unit-of-work-pattern-with-dapper/
///https://www.youtube.com/watch?v=GsP_dFuZKs8&ab_channel=RoelVandePaar
///https://hibernatingrhinos.com/products/nhprof/learn/#DoNotUseImplicitTransactions
///

using Learn.Dapper;
using Learn.EF;
using Learn.NHibernate;
using RepositoryLearn.Models;

var efContext = new EFLearnContext();
using(EFUnitOfWork<EFLearnContext> uow = new(efContext))
{
    //a.Companies.Create(new Company { Id = 1, Name = "First" });
    //a.Save();
    Console.WriteLine("EF existing:");
    foreach(var item in await uow.Companies.GetAsync())
    {
        Console.WriteLine($"{item.Id}: {item.Name}");
    }

    var item1 = uow.Companies.FindById(1);
    item1.Name = item1.Name + "1";
    uow.Companies.Update(item1);
    Console.WriteLine("EF after change before save:");
    foreach(var item in await uow.Companies.GetAsync())
    {
        Console.WriteLine($"{item.Id}: {item.Name}");
    }
    uow.Save();
    Console.WriteLine("EF after save:");
    foreach(var item in await uow.Companies.GetAsync())
    {
        Console.WriteLine($"{item.Id}: {item.Name}");
    }
}

//using var dapperUow = new DapperUnitOfWork();
//Console.WriteLine("Dapper existing:");
//foreach (var item in await dapperUow.Companies.GetAsync())
//{
//    Console.WriteLine($"{item.Id}: {item.Name}");
//}
//await dapperUow.Companies.CreateAsync(new Company { Id = 7, Name = "Company2" });
////Here we have different result from EF!
////Because we are in transaction in dapper and working with what changed already?
//Console.WriteLine("Dapper after change before save:");
//foreach(var item in await dapperUow.Companies.GetAsync()) 
//{
//    Console.WriteLine($"{item.Id}: {item.Name}");
//}
//dapperUow.Save();
//Console.WriteLine("Dapper after save:");
//foreach(var item in await dapperUow.Companies.GetAsync())
//{
//    Console.WriteLine($"{item.Id}: {item.Name}");
//}
//var item11 = dapperUow.Companies.FindById(1);
//item11.Name = item11.Name + "1";
//dapperUow.Companies.Update(item11);
//Console.WriteLine("Dapper after update:");
//foreach(var item in await dapperUow.Companies.GetAsync())
//{
//    Console.WriteLine($"{item.Id}: {item.Name}");
//}
//dapperUow.Rollback();
//Console.WriteLine("Dapper after rollback:");
//foreach(var item in await dapperUow.Companies.GetAsync())
//{
//    Console.WriteLine($"{item.Id}: {item.Name}");
//}


//using NHibernateUnitOfWork nuow = new();
//Console.WriteLine("Nhiber existing:");
//foreach(var item in await nuow.Companies.GetAsync())
//{
//    Console.WriteLine($"{item.Id}: {item.Name}");
//}

//var company = await nuow.Companies.FindByIdAsync(1);
//company.Name += 3;
//await nuow.Companies.UpdateAsync(company);
//Console.WriteLine("Nhiber after update before save");
//foreach(var item in await nuow.Companies.GetAsync())
//{
//    Console.WriteLine($"{item.Id}: {item.Name}");
//}
//nuow.Commit();
//Console.WriteLine("Nhiber after save");
//foreach(var item in await nuow.Companies.GetAsync())
//{
//    Console.WriteLine($"{item.Id}: {item.Name}");
//}