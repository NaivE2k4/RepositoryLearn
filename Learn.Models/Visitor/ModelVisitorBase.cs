using RepositoryLearn.Models;

namespace Learn.Models.Visitor;

/// <summary>
/// This is just a pattern training. Dont do this at home=)
/// </summary>
public abstract class ModelVisitorBase
{
    public abstract void VisitCompany(Company company);
    public abstract void VisitPhone(Phone phone);
    public abstract void VisitNhibernateCompany(NHibernate.Company company);
    public abstract void VisitNhibernatePhone(NHibernate.Phone phone);
}
