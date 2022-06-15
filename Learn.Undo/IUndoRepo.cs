using Learn.Undo;

namespace Learn.Abstractions;

//marker
public interface IUndoRepo
{
    void UndoOperaton(UndoInfo undoInfo);
}
