using Learn.Abstractions;
using Learn.Undo;
using Microsoft.EntityFrameworkCore;
using RepositoryLearn.Models;

namespace Learn.EF;

//Я знаю что EF сам в себе реализует UOW и Repository
public class EFUnitOfWork<TContext> : IDisposable where TContext : DbContext
{
    private TContext _dBContext;
    private IGenericRepository<Company> _companies;
    private IGenericRepository<Phone> _phones;
    private UndoInfo _undoInfo;
    //private Dictionary<Type, IGenericRepository> Repos;

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
        _undoInfo = new UndoInfo();
        _companies = new EFGenericRepository<Company>(_dBContext, _undoInfo);
        _phones = new EFGenericRepository<Phone>(_dBContext, _undoInfo);
    }

    public void Save()
    { 
        _dBContext.SaveChanges();
    }

    public void Undo()
    {
        switch(_undoInfo.EntityType)
        {
            //case typeof(Company):
            //    break;
            //case
        }
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
}
