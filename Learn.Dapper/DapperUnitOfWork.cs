using Learn.Abstractions;
using Learn.Undo;
using RepositoryLearn.Models;
using System.Data;
using System.Data.SQLite;

namespace Learn.Dapper;
/*
There are many variants in internet and SO.
Most agree, that technically, when working with databases, UOW is mostly equals DB transaction (being business transaction)
Philosophy in this class is this:
-Repository is a transient object, a container for queries
-UOW is like dbContext
-So user uses UOW, and if there is a need for more than one business transaction on this object, 
we should pass new transaction to repositories and then to dapper methods.
But it should be transparent to user. So i decided to try make repositories being created everytime on get{}
with existing transaction object
-Creating new objects here violates SOLID T_T
 */
public class DapperUnitOfWork : IDisposable, IUnitOfWork
{
    private bool _disposedValue;
    private IDbTransaction? _dbTransaction;
    private IDbConnection _dbConnection;
    private UowUndoCollection _undoCollection;

    public DapperCompanyRepository Companies
    {
        get
        {
            CheckAndStart();
            return new DapperCompanyRepository(_dbTransaction!, _undoCollection);
        }
    }
    public DapperPhoneRepository Phones
    {
        get
        {
            CheckAndStart();
            return new DapperPhoneRepository(_dbTransaction!, _undoCollection);
        }
    }

    private static Dictionary<Type, Type> objToRepoDict
        = new Dictionary<Type, Type>()
        {
            { typeof(Company), typeof(DapperCompanyRepository)},
            { typeof(Phone), typeof(DapperPhoneRepository)},
        };

    public IRepository GetRepo(Type T)
    {
        CheckAndStart();
        return (IRepository)Activator.CreateInstance(objToRepoDict[T], _dbTransaction, _undoCollection);
    }

    public DapperUnitOfWork()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var DbPath = Path.Join(path, "blogging.db");
        var connectionString = $"Data Source={DbPath}";
        _dbConnection = new SQLiteConnection(connectionString);
        _dbConnection.Open();
        _undoCollection = new UowUndoCollection();
    }
    private void CheckAndStart()
    {
        if(_dbTransaction == null)
        {
            Start();
        }
    }
    public void Start()
    { 
        _dbTransaction = _dbConnection.BeginTransaction();
    }

    public void Rollback()
    { 
        _dbTransaction?.Rollback();
        _dbTransaction?.Dispose();
        _dbTransaction = null;
    }

    public void Save()
    {
        _dbTransaction?.Commit();
        _dbTransaction?.Dispose();
        _dbTransaction = null;
    }

    public void Undo()
    {
        while (_undoCollection.CanUndo)
        {
            var undoItem = _undoCollection.UndoOne();
            var repo = GetRepo(undoItem.EntityType);
            repo.UndoOperaton(undoItem);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: освободить управляемое состояние (управляемые объекты)
                _dbTransaction?.Rollback();
                _dbConnection?.Close();

            }
            _dbTransaction?.Dispose();
            _dbConnection?.Dispose();
            // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
            // TODO: установить значение NULL для больших полей
            _disposedValue = true;
        }
    }

    // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
    ~DapperUnitOfWork()
    {
        // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }


}
