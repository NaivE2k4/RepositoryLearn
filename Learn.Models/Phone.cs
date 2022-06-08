namespace RepositoryLearn.Models;

public class Phone
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public int CompanyId { get; set; }
    public Company Company { get; set; }
}
