using Microsoft.EntityFrameworkCore;
using RepositoryLearn.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLearn
{
    public class UnitOfWork : IDisposable
    {
        private DbContext _dBContext = new DbContext(new DbContextOptionsBuilder().Options);
        private IGenericRepository<Company> companies;
        private IGenericRepository<Phone> phones;

        public IGenericRepository<Company> Companies
        { 
            get 
            {
                if (companies == null)
                    companies = new EFGenericRepository<Company>(_dBContext);
                return companies; 
            }
        }
        public IGenericRepository<Phone> Phones
        {
            get
            {
                if(phones == null)
                    phones = new EFGenericRepository<Phone>(_dBContext);
                return phones;
            }
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
}
