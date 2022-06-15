using Learn.Models.Visitor;

namespace Learn.Models.NHibernate;

public class Phone : IVisitableModel
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; }
    public virtual decimal Price { get; set; }

    public virtual int CompanyId { get; set; }
    public virtual Company Company { get; set; }

    public virtual void Accept(ModelVisitorBase visitor)
    {
        visitor.VisitNhibernatePhone(this);
    }
}
