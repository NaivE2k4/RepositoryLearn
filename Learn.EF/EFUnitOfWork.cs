using Learn.Abstractions;
using Learn.Undo;
using Microsoft.EntityFrameworkCore;
using RepositoryLearn.Models;

namespace Learn.EF;

//Я знаю что EF сам в себе реализует UOW и Repository
//Это просто для отработки...
public class EFUnitOfWork<TContext> : IDisposable, IUnitOfWork where TContext : DbContext
{
    private TContext _dBContext;
    private IGenericRepository<Company> _companies;
    private IGenericRepository<Phone> _phones;
    private readonly UowUndoCollection _undoCollection;
    private Dictionary<Type, IUndoRepo> Repos;

    public IGenericRepository<Company> Companies
    { 
        get 
        {
            return _companies;
        }
    }
    public IGenericRepository<Phone> Phones
    {
        get
        {
            return _phones;
        }
    }

    public EFUnitOfWork(TContext dBContext)
    {
        _dBContext = dBContext;
        _undoCollection = new();
        _companies = new EFGenericRepository<Company>(_dBContext, _undoCollection);
        _phones = new EFGenericRepository<Phone>(_dBContext, _undoCollection);

        Repos =
            new Dictionary<Type, IUndoRepo>
            {
                { typeof(Company), (IUndoRepo)_companies},
                { typeof(Phone), (IUndoRepo)_phones},
            };
    }

    public void Save()
    { 
        _dBContext.SaveChanges();
        _dBContext.ChangeTracker.Clear();
    }

    /// <summary>
    /// Undo all registered (saved or not) changes
    /// </summary>
    public void Undo()
    {
        
        while (_undoCollection.CanUndo)
        {
            var undoInfo = _undoCollection.UndoOne();
            var repo = Repos[undoInfo.EntityType!];
            repo.UndoOperaton(undoInfo);
        }
        Save();
    }

    private bool disposed = false;

    public virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _dBContext.Dispose();
            }
            this.disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void Start()
    {
        _dBContext.ChangeTracker.Clear();
    }

    public void Rollback()
    {
        _dBContext.ChangeTracker.Clear();
    }
}
