using Learn.Abstractions;
using Learn.Undo;
using Microsoft.Data.Sqlite;
using RepositoryLearn.Models;
using System.Data;

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

    public GenericUndoRepository<Company> Companies
    {
        get
        {
            CheckAndStart();
            var repo = new DapperCompanyRepository(_dbTransaction!);
            return new GenericUndoRepository<Company>(repo, _undoCollection);
        }
    }
    public GenericUndoRepository<Phone> Phones
    {
        get
        {
            CheckAndStart();
            var repo = new DapperPhoneRepository(_dbTransaction!);
            return new GenericUndoRepository<Phone>(repo, _undoCollection);
        }
    }

    private static Dictionary<Type, (Type, Type)> objToRepoDict
        = new Dictionary<Type, (Type, Type)>()
        {
            { typeof(Company), (typeof(DapperCompanyRepository), typeof(GenericUndoRepository<Company>))},
            { typeof(Phone), (typeof(DapperPhoneRepository), typeof(GenericUndoRepository<Phone>))},
        };

    public IUndoRepo GetRepo(Type type)
    {
        CheckAndStart();
        var repo = (IRepository) Activator.CreateInstance(objToRepoDict[type].Item1, _dbTransaction);
        var repo2 = (IUndoRepo) Activator.CreateInstance(objToRepoDict[type].Item2, repo, _undoCollection);
        return repo2;
    }

    public DapperUnitOfWork()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var DbPath = Path.Join(path, "blogging.db");
        var connectionString = $"Data Source={DbPath}";
        _dbConnection = new SqliteConnection(connectionString);
        _dbConnection.Open();
        _undoCollection = new UowUndoCollection();
    }

    public DapperUnitOfWork(string connstring)
    {
        _dbConnection = new SqliteConnection(connstring);
        _dbConnection.Open();
        _undoCollection = new UowUndoCollection();
    }

    public DapperUnitOfWork(SqliteConnection connection)
    {
        _dbConnection = connection;
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
        while(_undoCollection.CanUndo)
        {
            var undoItem = _undoCollection.UndoOne();
            var repo = GetRepo(undoItem.EntityType!);
            repo.UndoOperaton(undoItem);
        }
        Save();
    }

    protected virtual void Dispose(bool disposing)
    {
        if(!_disposedValue)
        {
            if(disposing)
            {
                _dbTransaction?.Rollback();
                _dbConnection?.Close();

            }
            _dbTransaction?.Dispose();
            _dbConnection?.Dispose();
            _disposedValue = true;
        }
    }

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
