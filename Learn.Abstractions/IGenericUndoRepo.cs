namespace Learn.Abstractions
{
    public  interface IGenericUndoRepo<TEntity> : IGenericRepository<TEntity>, IRepository where TEntity : class
    {

    }
}
