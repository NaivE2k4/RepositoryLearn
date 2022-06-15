using Learn.Models.Visitor;

namespace RepositoryLearn.Models;

public class Company : IVisitableModel, ICloneable
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public void Accept(ModelVisitorBase visitor)
    {
        visitor.VisitCompany(this);
    }

    public object Clone()
    {
        return new Company
        {
            Id = this.Id,
            Name = this.Name
        };
    }
}
