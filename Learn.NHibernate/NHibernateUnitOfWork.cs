using Learn.Abstractions;
using Learn.Models.NHibernate;
using Learn.Undo;
using NHibernate;
using NHibernate.Cfg;
using System.Data.Common;

namespace Learn.NHibernate;

public class NHibernateUnitOfWork : IDisposable, IUnitOfWork
{
    private readonly Configuration _configuration;
    private readonly ISessionFactory _sessionFactory;
    private ITransaction _transaction;
    private ISession _session;
    private readonly DbConnection _connection;
    private readonly UowUndoCollection _undoCollection = new();
    private bool _disposedValue;
    private static readonly Dictionary<Type, (Type, Type)> _entityToRepo =
        new()
        {
            { typeof(Company), (typeof(NHibernateCompanyRepository), typeof(GenericUndoRepository<Company>) )},
            { typeof(Phone), (typeof(NHibernatePhoneRepository), typeof(GenericUndoRepository<Phone>)) },
        };

    public GenericUndoRepository<Phone> Phones
    {
        get
        {
            CheckAndStart();
            var repo = new NHibernatePhoneRepository(_session);
            return new GenericUndoRepository<Phone>(repo, _undoCollection);
        }
    }
    public GenericUndoRepository<Company> Companies
    {
        get
        {
            CheckAndStart();
            var repo = new NHibernateCompanyRepository(_session);
            return new GenericUndoRepository<Company>(repo, _undoCollection);
        }
    }
    public NHibernateUnitOfWork()
    {
        var folder = System.Environment.SpecialFolder.LocalApplicationData;
        var path = System.Environment.GetFolderPath(folder);
        var DbPath = Path.Join(path, "blogging.db");
        var connectionString = $"Data Source={DbPath}";
        _configuration = new Configuration();
        _configuration.Configure();
        _configuration.SetProperty("connection.connection_string", connectionString);
        _configuration.AddClass(typeof(Company));
        _configuration.AddClass(typeof(Phone));
        //_configuration.Cache(props => props.UseQueryCache = false);

        _sessionFactory = _configuration.BuildSessionFactory();
    }

    public NHibernateUnitOfWork(string connString)
    {
        _configuration = new Configuration();
        _configuration.Configure();
        _configuration.SetProperty("connection.connection_string", connString);
        _configuration.AddClass(typeof(Company));
        _configuration.AddClass(typeof(Phone));
        //_configuration.Cache(props => props.UseQueryCache = false);

        _sessionFactory = _configuration.BuildSessionFactory();
    }
    public NHibernateUnitOfWork(DbConnection connection)
    {
        _configuration = new Configuration();
        _configuration.Configure();
        _configuration.SetProperty("connection.connection_string", connection.ConnectionString);
        _configuration.AddClass(typeof(Company));
        _configuration.AddClass(typeof(Phone));
        //_configuration.Cache(props => props.UseQueryCache = false);
        _sessionFactory = _configuration.BuildSessionFactory();
        _connection = connection;
    }

    private void CheckAndStart()
    {
        if(_transaction == null || !_transaction.IsActive)
        {
            Start();
        }
    }
    public void Start()
    {
        if(_connection != null)
            _session = _sessionFactory.WithOptions().Connection(_connection).OpenSession();
        else
            _session = _sessionFactory.OpenSession();
        //_session.CacheMode = CacheMode.Ignore;
        //_session.DefaultReadOnly = true;
        _transaction = _session.BeginTransaction();
    }

    public void Save()
    {
        _transaction.Commit();
        _session.Close();
    }

    public void Rollback()
    {
        if(_transaction.IsActive)
            _transaction.Rollback();
        _session?.Close();
    }

    public void Undo()
    {
        while(_undoCollection.CanUndo)
        {
            var undoItem = _undoCollection.UndoOne();
            var repoType = _entityToRepo[undoItem.EntityType];
            var repo = (IRepository) Activator.CreateInstance(repoType.Item1, _session);
            var undoRepo = (IUndoRepo) Activator.CreateInstance(repoType.Item2, repo, _undoCollection);
            undoRepo.UndoOperaton(undoItem);
        }
        Save();
    }

    public virtual void Dispose(bool disposing)
    {
        if(!_disposedValue)
        {
            if(disposing)
            {
                // TODO: освободить управляемое состояние (управляемые объекты)
                Rollback();
            }

            // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
            // TODO: установить значение NULL для больших полей
            _transaction?.Dispose();
            _session?.Dispose();
            _disposedValue = true;
        }
    }

    // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
    ~NHibernateUnitOfWork()
    {
        // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        Dispose(disposing: false);
    }

    void IDisposable.Dispose()
    {
        // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

}
