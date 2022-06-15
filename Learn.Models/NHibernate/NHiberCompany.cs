using Learn.Models.Visitor;

namespace Learn.Models.NHibernate;

public class Company : IVisitableModel
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; }

    public virtual void Accept(ModelVisitorBase visitor)
    {
        visitor.VisitNhibernateCompany(this);
    }
}
