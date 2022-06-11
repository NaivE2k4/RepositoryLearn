using Learn.Abstractions;
using Learn.NHibernate.Models;
using NHibernate;
using NHibernate.Cfg;

namespace Learn.NHibernate;

public class NHibernateUnitOfWork : IDisposable, IUnitOfWork
{
    private Configuration _configuration;
    private ISessionFactory _sessionFactory;
    private ITransaction _transaction;
    private ISession _session;
    private bool _disposedValue;

    public NHibernatePhoneRepository Phones 
    {
        get
        {
            CheckAndStart();
            return new NHibernatePhoneRepository(_session);
        }
    }
    public NHibernateCompanyRepository Companies 
    {
        get
        {
            CheckAndStart();
            return new NHibernateCompanyRepository(_session);
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
        _sessionFactory = _configuration.BuildSessionFactory();
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
        _session = _sessionFactory.OpenSession();
        _transaction = _session.BeginTransaction();
    }

    public void Save()
    {
        _transaction.Commit();
        _session.Close();
    }

    public void Rollback()
    {
        _transaction.Rollback();
        _session.Close();
    }

    public void Undo()
    {
        throw new NotImplementedException();
    }

    protected virtual void Dispose(bool disposing)
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
