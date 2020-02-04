using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Constellation;


[System.Serializable]
public class NodeWindow
{
    public List<NodeView> Nodes;
    public List<NodeView> SelectedNodes;
    public NodeEditorBackground background;
    bool mousePressed = false;
    Vector2 mouseClickStartPosition = Vector2.zero;
    public Vector2 ScrollPosition = Vector2.zero;
    public delegate void RequestRepaint();
    const float nodeTitleHeight = 20;
    const float nodeDeleteSize = 15;
    const float resizeButtonSize = 10;
    private enum EventsScope { Generic, Resizing, Dragging };
    private EventsScope currentEventScope = EventsScope.Generic;
    private string editorPath = "Assets/Constellation/Editor/EditorAssets/";


    public NodeWindow(string _editorPath)
    {
        var backgroundTexture = AssetDatabase.LoadAssetAtPath(editorPath + "background.png", typeof(Texture2D)) as Texture2D;
        background = new NodeEditorBackground(backgroundTexture);
        editorPath = _editorPath;
        SelectedNodes = new List<NodeView>();
        Nodes = new List<NodeView>();
    }

    public void AddNode(string nodeName)
    {
        const float nodeSize = 100;
        Nodes.Add(new NodeView(0, 0, nodeName, nodeSize, nodeSize));
    }

    public void UpdateGUI(RequestRepaint requestRepaint, float windowSizeX, float windowSiseY)
    {
        ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, GUILayout.Width(windowSizeX - 300), GUILayout.Height(windowSiseY));
        background.DrawBackgroundGrid(windowSizeX, windowSiseY, 0, 0, Color.white);
        Event e = Event.current;
        if (e.type == EventType.MouseDown && Event.current.button == 0)
        {
            mouseClickStartPosition = e.mousePosition;
            mousePressed = true;
        }
        if (e.type == EventType.MouseUp && Event.current.button == 0)
        {
            currentEventScope = EventsScope.Generic;
            mousePressed = false;
            for (var i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].LockNodeSize();
                Nodes[i].LockNodePosition();
            }
        }


        switch (currentEventScope)
        {
            case EventsScope.Generic:
                UpdateGenericEvents(requestRepaint, e);
                break;
            case EventsScope.Resizing:
                UpdateResizeEvents(requestRepaint, e);
                break;
            case EventsScope.Dragging:
                UpdateDragEvents(requestRepaint, e);
                break;

        }

        UpdateNodesVisual(e);
        EditorGUILayout.EndScrollView();
    }

    private void UpdateNodesVisual(Event e)
    {
        //Read in reverse so first element in in front;
        for (int i = Nodes.Count - 1; i >= 0; i--)
        {
            var nodeSizeX = Nodes[i].GetSizeX();
            var nodeSizeY = Nodes[i].GetSizeY();
            var nodePositionX = Nodes[i].GetPositionX();
            var nodePositionY = Nodes[i].GetPositionY();
            float positionOffsetX = nodeSizeX * 0.5f;
            float positionOffsetY = nodeSizeY * 0.5f;
            var nodeRect = new Rect(nodePositionX, nodePositionY, nodeSizeX, nodeSizeY);
            var nodeTitleRect = new Rect(nodePositionX, nodePositionY, nodeSizeX, nodeTitleHeight);
            var deleteRect = new Rect(nodePositionX + (nodeSizeX - nodeDeleteSize), nodePositionY, nodeDeleteSize, nodeDeleteSize);
            var questionRect = new Rect(nodePositionX + (nodeSizeX - nodeDeleteSize - nodeDeleteSize), nodePositionY, nodeDeleteSize, nodeDeleteSize);
            var resizeRect = new Rect(nodePositionX + nodeSizeX - resizeButtonSize, nodePositionY + nodeSizeY - resizeButtonSize, resizeButtonSize, resizeButtonSize);
            GUI.Box(nodeRect, "", GUI.skin.GetStyle("Button"));
            GUI.Label(nodeTitleRect, Nodes[i].GetName());
            if (nodeRect.Contains(e.mousePosition))
            {
                GUI.color = new Color(1, 0.5f, 0.5f);
                GUI.Button(deleteRect, "X");
                GUI.color = new Color(1, 0.7f, 0.1f); ;
                GUI.Button(questionRect, "?");
                GUI.color = Color.gray;
                GUI.Button(resizeRect, "");
            }
            GUI.color = Color.white;
        }
    }

    private void UpdateResizeEvents(RequestRepaint requestRepaint, Event e)
    {
        for (var i = 0; i < SelectedNodes.Count; i++)
        {
            var nodeRect = GetNodeRect(SelectedNodes[i], out float positionOffsetX, out float positionOffsetY);
            var deleteRect = GetDeleteRect(SelectedNodes[i]);
            var questionRect = GetQuestionRect(SelectedNodes[i]);
            var resizeRect = GetResizeRect(SelectedNodes[i]);
            SelectedNodes[i].UpdateNodeSize((e.mousePosition.x - mouseClickStartPosition.x) + SelectedNodes[i].GetPreviousNodeSizeX(), (e.mousePosition.y - mouseClickStartPosition.y) + SelectedNodes[i].GetPreviousNodeSizeY());
        }
        requestRepaint();
    }

    private void UpdateDragEvents(RequestRepaint requestRepaint, Event e)
    {
        for (var i = 0; i < SelectedNodes.Count; i++)
        {
            var nodeRect = GetNodeRect(SelectedNodes[i], out float positionOffsetX, out float positionOffsetY);
            var deleteRect = GetDeleteRect(SelectedNodes[i]);
            var questionRect = GetQuestionRect(SelectedNodes[i]);
            var resizeRect = GetResizeRect(SelectedNodes[i]);
            SelectedNodes[i].SetPosition((e.mousePosition.x - mouseClickStartPosition.x) + SelectedNodes[i].GetPreviousNodePositionX(), (e.mousePosition.y - mouseClickStartPosition.y) + SelectedNodes[i].GetPreviousNodePositionY());
        }
        requestRepaint();
    }

    private void UpdateGenericEvents(RequestRepaint requestRepaint, Event e)
    {
        for (var i = 0; i < Nodes.Count; i++)
        {
            var nodeRect = GetNodeRect(Nodes[i], out float positionOffsetX, out float positionOffsetY);
            var deleteRect = GetDeleteRect(Nodes[i]);
            var questionRect = GetQuestionRect(Nodes[i]);
            var resizeRect = GetResizeRect(Nodes[i]);

            if (nodeRect.Contains(e.mousePosition) && mousePressed)
            {
                requestRepaint();
                if (e.control)
                {
                    //Debug.Log("Add node to selection");
                    SelectedNodes.Add(Nodes[i]);
                }
                else
                {
                    //Debug.Log("Add node to selection and clear");
                    SelectedNodes.Clear();
                    SelectedNodes.Add(Nodes[i]);
                }

                if (deleteRect.Contains(e.mousePosition))
                {
                    Nodes.Remove(Nodes[i]);
                    return;
                }

                if (questionRect.Contains(e.mousePosition))
                {
                    //Debug.Log("Help");
                    return;
                }

                if (resizeRect.Contains(e.mousePosition))
                {
                    currentEventScope = EventsScope.Resizing;
                    Debug.Log("Resize");
                    return;
                }

                currentEventScope = EventsScope.Dragging;
                return;
            }
        }
        if (mousePressed)
        {
            Debug.Log("Clearing");
            SelectedNodes.Clear();
        }
    }

    private Rect GetNodeRect(NodeView node, out float positionOffsetX, out float positionOffsetY)
    {
        var rect = new Rect(node.GetPositionX(), node.GetPositionY(), node.GetSizeX(), node.GetSizeY());
        positionOffsetX = node.GetSizeX() * 0.5f;
        positionOffsetY = node.GetSizeY() * 0.5f;
        return rect;
    }

    private Rect GetDeleteRect(NodeView node)
    {
        return new Rect(node.GetPositionX() + (node.GetSizeX() - nodeDeleteSize), node.GetPositionY(), nodeDeleteSize, nodeDeleteSize);
    }

    private Rect GetQuestionRect(NodeView node)
    {
        return new Rect(node.GetPositionX() + (node.GetSizeX() - nodeDeleteSize - nodeDeleteSize), node.GetPositionY(), nodeDeleteSize, nodeDeleteSize);
    }

    private Rect GetResizeRect(NodeView node)
    {
        return new Rect(node.GetPositionX() + node.GetSizeX() - resizeButtonSize, node.GetPositionY() + node.GetSizeY() - resizeButtonSize, resizeButtonSize, resizeButtonSize);
    }
}
