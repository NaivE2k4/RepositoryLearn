///
///Это просто личный проект для практики разных базовых штук по работе с данными
///В нем нет чистоты кода или технического совершенства - просто отработки концепций и идей
///Ссылки:
///
///https://www.youtube.com/watch?v=3yPpL1rEK9o
///https://www.youtube.com/watch?v=oP_OUiIK4Rc
///https://stackoverflow.com/questions/42957140/repository-design-pattern-with-dapper
///https://dejanstojanovic.net/aspnet/2021/november/unit-of-work-pattern-with-dapper/
///

using Learn.Dapper;
using Learn.EF;


var efContext = new EFLearnContext();
using (EFUnitOfWork<EFLearnContext> uow = new(efContext))
{
    //a.Companies.Create(new Company { Id = 1, Name = "First" });
    //a.Save();
    foreach (var item in await uow.Companies.GetAsync())
    {
        Console.WriteLine($"{item.Id}: {item.Name}");
    }

    var item1 = uow.Companies.FindById(1);
    item1.Name = item1.Name + "1";
    uow.Companies.Update(item1);
    foreach (var item in await uow.Companies.GetAsync())
    {
        Console.WriteLine($"{item.Id}: {item.Name}");
    }
    uow.Save();
    foreach (var item in await uow.Companies.GetAsync())
    {
        Console.WriteLine($"{item.Id}: {item.Name}");
    }
}
using var dapperUow = new DapperUnitOfWork();
var dapperCompanyRepo = dapperUow.Companies;
foreach (var item in await dapperCompanyRepo.GetAsync())
{
    Console.WriteLine($"{item.Id}: {item.Name}");
}