namespace Learn.Abstractions
{
    public interface IUnitOfWork
    {
        void Start();
        void Save();
        void Rollback();
        void Undo();
    }
}
