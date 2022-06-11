using Learn.Undo;

namespace Learn.Abstractions
{
    public interface IRepository
    {//marker interface
        void UndoOperaton(UndoInfo undoInfo);
    }
}
