namespace Learn.NHibernate.Models;

public class Phone
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; }
    public virtual decimal Price { get; set; }

    public virtual int CompanyId { get; set; }
    public virtual Company Company { get; set; }
}
