using NHibernate;
using NHibernate.Cfg;

namespace Learn.NHibernate;

public class NHibernateUnitOfWork : IDisposable
{
    private Configuration _configuration;
    private ISessionFactory _sessionFactory;
    private ITransaction _transaction;
    private ISession _session;
    private bool _disposedValue;

    public NHibernatePhoneRepository Phones { get; set; }
    public NHibernateCompanyRepository Companies { get; set; }
    public NHibernateUnitOfWork()
    {
        var folder = System.Environment.SpecialFolder.LocalApplicationData;
        var path = System.Environment.GetFolderPath(folder);
        var DbPath = Path.Join(path, "blogging.db");
        var connectionString = $"Data Source={DbPath}";
        _configuration = new Configuration();
        _configuration.SetProperty("connection.connection_string", connectionString);
        _configuration.Configure();
        _sessionFactory = _configuration.BuildSessionFactory();
    }

    public void Start()
    {
        _session = _sessionFactory.OpenSession();
        _transaction = _session.BeginTransaction();
    }

    public void Commit()
    {
        _transaction.Commit();
        _session.Close();
    }

    public void Rollback()
    {
        _transaction.Rollback();
        _session.Close();
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
