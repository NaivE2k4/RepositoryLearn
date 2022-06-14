namespace Learn.Models.Visitor;
/// <summary>
/// This is just a pattern training. Dont do this at home=)
/// </summary>
public interface IVisitableModel
{
    void Accept(ModelVisitorBase visitor);
}
