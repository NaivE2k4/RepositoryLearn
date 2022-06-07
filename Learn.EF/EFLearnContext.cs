using Microsoft.EntityFrameworkCore;
using RepositoryLearn.Models;

namespace Learn.EF
{
    public class EFLearnContext : DbContext
    {
        public DbSet<Company> Companies { get; set; }
        public DbSet<Phone> Phones { get; set; }
    }
}