using RepositoryLearn.Models;

namespace Learn.Models.Visitor
{
    public class GetIdVisitor : ModelVisitorBase
    {
        public int Result { get; private set; }
        public override void VisitCompany(Company company)
        {
            Result = company.Id;
        }
        public override void VisitPhone(Phone phone)
        {
            Result = phone.Id;
        }
        public override void VisitNhibernateCompany(NHibernate.Company company)
        {
            Result = company.Id;
        }
        public override void VisitNhibernatePhone(NHibernate.Phone phone)
        {
            Result = phone.Id;
        }
    }
}
