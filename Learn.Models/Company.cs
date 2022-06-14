using Learn.Models.Visitor;

namespace RepositoryLearn.Models;

public class Company : IVisitableModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    public void Accept(ModelVisitorBase visitor)
    {
        visitor.VisitCompany(this);
    }
}
