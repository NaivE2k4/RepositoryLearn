/*
 Это просто проект для практики разных базовых штук по работе с данными
 */

using Learn.EF;
using RepositoryLearn.Models;

var efContext = new EFLearnContext();
EFUnitOfWork<EFLearnContext> a = new(efContext);
a.Companies.Create(new Company { Id = 1, Name = "First" });
a.Save();