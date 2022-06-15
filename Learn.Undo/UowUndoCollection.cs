namespace Learn.Undo;

public class UowUndoCollection
{
    private readonly List<UndoInfo> _undos = new();
    private int _currentItem = -1;

    public void Clear()
    {
        _undos.Clear();
        _currentItem = -1;
    }
    public void Add(UndoInfo info)
    {
        _currentItem++;
        if(_currentItem < _undos.Count)
            _undos[_currentItem] = info;
        else
            _undos.Add(info);
        //Ideally we should clear next items if they exists
        if(_currentItem < _undos.Count - 1) //The pointer is not on a last element
            _undos.RemoveRange(_currentItem + 1, _undos.Count - 1 - _currentItem); //Check it!
    }

    public void Add(int id, Type entityType, UndoOpType opType, object? prevState)
    {
        var undoInfo = new UndoInfo { Id = id, EntityType = entityType, OpType = opType, PrevState = prevState };
        Add(undoInfo);
    }

    public bool CanUndo => _currentItem > -1;

    /// <summary>
    /// Возвращает итем и двигает указатель назад. Итем остается на месте на случай Redo
    /// </summary>
    /// <returns>UndoInfo.Empty if cant undo or item</returns>
    public UndoInfo UndoOne()
    {
        if(!CanUndo)
            return UndoInfo.Empty;
        return _undos[_currentItem--];
    }

    public bool CanRedo()
    {
        return _currentItem > -1 && _currentItem < _undos.Count - 1;
    }

    /// <summary>
    /// Moves one step forward if possible and returns element there
    /// </summary>
    /// <returns>UndoInfo.Empty if cant redo or item</returns>
    public UndoInfo RedoOne()
    {
        if(!CanRedo())
            return UndoInfo.Empty;
        return _undos[++_currentItem];

    }
}
