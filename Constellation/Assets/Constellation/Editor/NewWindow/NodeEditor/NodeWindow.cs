using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Constellation;


[System.Serializable]
public class NodeWindow
{
    public List<NodeView> Nodes;
    public List<NodeView> SelectedNodes;
    public LinksView Links;
    public NodeEditorBackground background;
    public NodesFactory NodeFactory;
    public ConstellationScript ConstellationScript;
    private NodeView SetOnTop;
    bool mousePressed = false;
    Vector2 mouseClickStartPosition = Vector2.zero;
    public Vector2 ScrollPosition = Vector2.zero;
    public delegate void RequestRepaint();

    private enum EventsScope { Generic, Resizing, Dragging };
    private EventsScope currentEventScope = EventsScope.Generic;
    private string editorPath = "Assets/Constellation/Editor/EditorAssets/";


    public NodeWindow(string _editorPath, ConstellationScript _constellationScript)
    {
        var backgroundTexture = AssetDatabase.LoadAssetAtPath(editorPath + "background.png", typeof(Texture2D)) as Texture2D;
        background = new NodeEditorBackground(backgroundTexture);
        editorPath = _editorPath;
        SelectedNodes = new List<NodeView>();
        Nodes = new List<NodeView>();
        ConstellationScript = _constellationScript;
        Links = new LinksView(ConstellationScript);
        NodeFactory = new NodesFactory(ConstellationScript.ScriptAssembly.GetAllScriptData());

        foreach(var node in _constellationScript.GetNodes())
        {
            DisplayNode(node);
        }
    }

    public void DisplayNode(NodeData node)
    {
        Nodes.Add(new NodeView(node));
    }

    public void AddNode(string nodeName, string nodeNamespace)
    {
        const float nodeSize = 100;
        var newNode = NodeFactory.GetNode(nodeName, nodeNamespace);
        var nodeData = new NodeData(newNode);
        nodeData.SizeX = nodeSize;
        nodeData.SizeY = nodeSize;
        nodeData = ConstellationScript.AddNode(nodeData);
        nodeData.XPosition = 0;
        nodeData.YPosition = 0;
        nodeData.SizeX = 100;
        nodeData.SizeY = 100;
        Nodes.Add(new NodeView(nodeData));
    }

    public void RemoveNode(NodeData node)
    {
        foreach(var nodeView in Nodes)
        {
            if(nodeView.NodeData.Guid == node.Guid)
            {
                SelectedNodes.Remove(nodeView);
                Nodes.Remove(nodeView);
                ConstellationScript.RemoveNode(nodeView.NodeData);
                return;
            }
        }
    }

    public void Draw(RequestRepaint requestRepaint, float windowSizeX, float windowSiseY)
    {
        ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, GUILayout.Width(windowSizeX - 300), GUILayout.Height(windowSiseY));
        background.DrawBackgroundGrid(windowSizeX, windowSiseY, 0, 0, Color.white);
        Event e = Event.current;
        var mouseJustRelease = false;
        if (e.type == EventType.MouseUp && Event.current.button == 0 && mousePressed == true)
        {
            mouseJustRelease = true;
            mousePressed = false;
        }
        else if (e.type == EventType.MouseDown && Event.current.button == 0)
        {
            mouseClickStartPosition = e.mousePosition;
            mousePressed = true;
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

        //Needs to be called after the event scope otherwise quit button event is overriden by the node drag event
        if (mouseJustRelease)
        {
            currentEventScope = EventsScope.Generic;
            for (var i = 0; i < Nodes.Count; i++)
            {
                Nodes[i].LockNodeSize();
                Nodes[i].LockNodePosition();
            }
        }
        DrawNodes(e);
        Links.DrawLinks();
        EditorGUILayout.EndScrollView();
    }

    private void DrawNodes(Event e)
    {
        //Read in reverse so first element in in front;
        for (int i = Nodes.Count - 1; i >= 0; i--)
        {
            Nodes[i].DrawNode(e);
        }
    }

    private void UpdateResizeEvents(RequestRepaint requestRepaint, Event e)
    {
        for (var i = 0; i < SelectedNodes.Count; i++)
        {
            SelectedNodes[i].UpdateNodeSize((e.mousePosition.x - mouseClickStartPosition.x) + SelectedNodes[i].GetPreviousNodeSizeX(), (e.mousePosition.y - mouseClickStartPosition.y) + SelectedNodes[i].GetPreviousNodeSizeY());
        }
        requestRepaint();
    }

    private void UpdateDragEvents(RequestRepaint requestRepaint, Event e)
    {
        for (var i = 0; i < SelectedNodes.Count; i++)
        {
            SelectedNodes[i].SetPosition((e.mousePosition.x - mouseClickStartPosition.x) + SelectedNodes[i].GetPreviousNodePositionX(), (e.mousePosition.y - mouseClickStartPosition.y) + SelectedNodes[i].GetPreviousNodePositionY());
        }
        requestRepaint();
    }

    private void UpdateGenericEvents(RequestRepaint requestRepaint, Event e)
    {
        for (var i = 0; i < Nodes.Count; i++)
        {
            var nodeRect = Nodes[i].GetNodeRect(out float positionOffsetX, out float positionOffsetY);
            var deleteRect = Nodes[i].GetDeleteRect();
            var questionRect = Nodes[i].GetQuestionRect();
            var resizeRect = Nodes[i].GetResizeRect();

            if (nodeRect.Contains(e.mousePosition) && mousePressed)
            {
                requestRepaint();
                if (e.control)
                {
                    //Debug.Log("Add node to selection");
                    SelectedNodes.Add(Nodes[i]);
                    SetNodeToFirst(Nodes[i]);
                }
                else
                {
                    SelectedNodes.Clear();
                    SelectedNodes.Add(Nodes[i]);
                }

                for(var j = 0; j < Nodes[i].GetInputs().Length; j++)
                {
                    var inputRect = Nodes[i].GetInputRect(j);
                    if(inputRect.Contains(e.mousePosition))
                        Links.AddLinkFromInput(Nodes[i].GetInputs()[j]);
                }

                for (var j = 0; j < Nodes[i].GetOutputs().Length; j++)
                {
                    var outputRect = Nodes[i].GetOuptputRect(j);
                    if(outputRect.Contains(e.mousePosition))
                        Links.AddLinkFromOutput(Nodes[i].GetOutputs()[j]);
                }

                if (deleteRect.Contains(e.mousePosition))
                {
                    RemoveNode(Nodes[i].NodeData);
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
                    return;
                }
                SetNodeToFirst(Nodes[i]);
                currentEventScope = EventsScope.Dragging;
                return;
            }
        }
        if (mousePressed)
        {
            SelectedNodes.Clear();
        }
    }

    private void SetNodeToFirst(NodeView node)
    {
        Nodes.Remove(node);
        Nodes.Insert(0,node);
    }
}
