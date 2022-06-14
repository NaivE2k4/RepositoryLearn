using Learn.Models.Visitor;

namespace RepositoryLearn.Models;

public class Phone : IVisitableModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public int CompanyId { get; set; }
    public Company Company { get; set; }

    public void Accept(ModelVisitorBase visitor)
    {
        visitor.VisitPhone(this);
    }
}
