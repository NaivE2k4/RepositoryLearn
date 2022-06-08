﻿using Learn.Abstractions;
using Microsoft.EntityFrameworkCore;
using RepositoryLearn.Models;

namespace Learn.EF;

//Я знаю что EF сам в себе реализует UOW и Repository
public class EFUnitOfWork<TContext> : IDisposable where TContext : DbContext
{
    private TContext _dBContext;
    private IGenericRepository<Company> _companies;
    private IGenericRepository<Phone> _phones;

    public IGenericRepository<Company> Companies
    { 
        get 
        {
            if (_companies == null)
                _companies = new EFGenericRepository<Company>(_dBContext);
            return _companies; 
        }
    }
    public IGenericRepository<Phone> Phones
    {
        get
        {
            if(_phones == null)
                _phones = new EFGenericRepository<Phone>(_dBContext);
            return _phones;
        }
    }

    public EFUnitOfWork(TContext dBContext)
    {
        _dBContext = dBContext;
    }

    public void Save()
    { 
        _dBContext.SaveChanges();
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
