using Learn.Abstractions;
using Learn.Undo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Learn.NHibernate
{
    public class NHibernateUndoRepository<TEntity> : IUndoRepo where TEntity : class
    {
        public void UndoOperaton(UndoInfo undoInfo)
        {
            throw new NotImplementedException();
        }
    }
}
