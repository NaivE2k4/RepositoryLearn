using Learn.Models.Visitor;

namespace Learn.Models.NHibernate;

public class Company : IVisitableModel, ICloneable
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; }

    public virtual void Accept(ModelVisitorBase visitor)
    {
        visitor.VisitNhibernateCompany(this);
    }

    public virtual object Clone()
    {
        return new Company
        {
            Id = this.Id,
            Name = this.Name
        };
    }
}
