namespace ConstellationEditor {
    public interface IUndoable {
        void AddAction ();
        void Undo ();
        void Redo();
    }
}