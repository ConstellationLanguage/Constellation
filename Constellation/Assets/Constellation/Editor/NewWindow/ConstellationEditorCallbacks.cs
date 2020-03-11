using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstellationEditorCallbacks
{
    public enum EditorEventType {NodeAdded, NodeMoved, LinkAdded, NodeDeleted, LinkDeleted, NodeResized}

    public delegate void RequestRepaint();
    public delegate void EditorEvents(EditorEventType editorEventType);
}
