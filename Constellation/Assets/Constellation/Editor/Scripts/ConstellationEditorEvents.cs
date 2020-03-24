namespace ConstellationEditor
{
    public class ConstellationEditorEvents
    {
        public enum EditorEventType { NodeAdded, NodeMoved, LinkAdded, NodeDeleted, LinkDeleted, NodeResized, HelpClicked, AddToUndo}
        public delegate void RequestRepaint();
        public delegate void EditorEvents(EditorEventType editorEventType, string message);
    }
}
