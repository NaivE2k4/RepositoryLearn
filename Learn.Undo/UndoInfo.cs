namespace Learn.Undo;

/// <summary>
/// Memento-style undo
/// </summary>
public class UndoInfo
{
    public UndoOpType OpType { get; set; } = UndoOpType.None;
    public Type? EntityType { get; set; } = null;
    public int Id { get; set; } = -1;
    public object? PrevState { get; set; } = null;

    public static UndoInfo Empty = new UndoInfo();
}