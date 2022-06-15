using Learn.Abstractions;

namespace Learn.Undo;
public  interface IGenericUndoRepo<TEntity> : IGenericRepository<TEntity>, IRepository where TEntity : class
{

}
