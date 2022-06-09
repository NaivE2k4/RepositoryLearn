using System.Data;
using System.Data.SQLite;

namespace Learn.Dapper;

public class DapperUnitOfWork : IDisposable
{
    private bool disposedValue;
    private IDbTransaction _dbTransaction;
    private IDbConnection _dbConnection;

    public DapperCompanyRepository CompanyRepository
    {
        get 
        {
            return new DapperCompanyRepository(_dbTransaction);
        }
    }
    public DapperPhoneRepository PhoneRepository 
    { 
        get 
        {
            return new DapperPhoneRepository(/*_dbTransaction*/);
        }
    }
    public DapperUnitOfWork()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        var DbPath = Path.Join(path, "blogging.db");
        var connectionString = $"Data Source={DbPath}";
        _dbConnection = new SQLiteConnection(connectionString);
    }
    public void Start()
    { 
        _dbTransaction = _dbConnection.BeginTransaction();
    }

    public void Rollback()
    { 
        _dbTransaction.Rollback();
    }

    public void Save()
    {
        _dbTransaction.Commit();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: освободить управляемое состояние (управляемые объекты)
            }

            // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
            // TODO: установить значение NULL для больших полей
            disposedValue = true;
        }
    }

    // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
    // ~DapperUnitOfWork()
    // {
    //     // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
