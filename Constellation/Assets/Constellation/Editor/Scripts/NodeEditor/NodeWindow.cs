using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Constellation;
using System.Linq;
using Constellation.Unity3D;

namespace ConstellationEditor
{
    public class NodeWindow
    {
        public List<NodeView> Nodes;
        public List<NodeView> SelectedNodes;
        public LinksView Links;
        public NodeEditorBackground background;
        public NodesFactory NodeFactory;
        public ConstellationScript ConstellationScript;
        public ConstellationEditorDataService EditorData;
        private NodeView SetOnTop;

        //private bool isInstance;
        bool mousePressed = false;
        bool mouseButtonDown = false;
        Vector2 mouseClickStartPosition = Vector2.zero;
        public Vector2 ScrollPosition = Vector2.zero;
        public float windowSizeX;
        public float windowSizeY;
        public float farNodeX = 0;
        public float farNodeY = 0;
        public Vector2 editorScrollSize;
        private enum EventsScope { Generic, Resizing, Dragging, EditingAttributes, Selecting };
        private EventsScope currentEventScope = EventsScope.Generic;
        private string editorPath = "Assets/Constellation/Editor/EditorAssets/";
        private string focusedNode = "";
        private Vector2 mousePosition;
        private ConstellationEditorRules constellationRules;

        public NodeWindow(string _editorPath, ConstellationEditorDataService _constellationEditorData, Vector2 windowSize, Vector2 scrollPosition)
        {
            constellationRules = new ConstellationEditorRules();
            farNodeX = windowSize.x;
            farNodeY = windowSize.y;
            editorScrollSize = new Vector2(farNodeX + 400, farNodeY + 400);
            ScrollPosition = scrollPosition;
            var backgroundTexture = AssetDatabase.LoadAssetAtPath(editorPath + "background.png", typeof(Texture2D)) as Texture2D;
            background = new NodeEditorBackground(backgroundTexture);
            editorPath = _editorPath;
            SelectedNodes = new List<NodeView>();
            Nodes = new List<NodeView>();
            EditorData = _constellationEditorData;
            ConstellationScript = EditorData.Script;
            Links = new LinksView(ConstellationScript, constellationRules);
            NodeFactory = new NodesFactory(ConstellationScript.ScriptAssembly.GetAllStaticScriptData());

            foreach (var node in ConstellationScript.GetNodes())
            {
                DisplayNode(node);
            }
        }

        public void SelectNodes(NodeData[] nodes)
        {
            ClearSelectedNodes();
            foreach (var windowNode in Nodes)
            {
                foreach (var node in nodes)
                {
                    if (node.Guid == windowNode.NodeData.Guid)
                    {
                        SelectedNodes.Add(windowNode);
                        windowNode.SelectedNode();
                    }
                }
            }
            foreach (var selectedNode in SelectedNodes)
            {
                SetNodeToFirst(selectedNode);
            }
        }

        void ClearSelectedNodes()
        {
            foreach (var selectedNode in SelectedNodes)
            {
                selectedNode.UnselectNode();
            }
            SelectedNodes.Clear();
        }

        public NodeView[] GetSelectedNodes()
        {
            return SelectedNodes.ToArray();
        }

        public void DisplayNode(NodeData node)
        {
            var nodeView = new NodeView(node);
            if (node.SizeX == 0 || node.SizeY == 0)
                nodeView.UpdateNodeSize(0, 0, EditorData.GetConstellationEditorConfig());
            Nodes.Add(nodeView);
        }

        public void AddNode(string nodeName, string nodeNamespace, ConstellationEditorEvents.EditorEvents editorEvent)
        {
            editorEvent(ConstellationEditorEvents.EditorEventType.AddToUndo, "Add node");
            var nodeData = constellationRules.AddNode(NodeFactory, nodeName, nodeNamespace, ConstellationScript.script);
            var newNodeView = new NodeView(nodeData);
            Nodes.Add(newNodeView);
            newNodeView.UpdateNodeSize(0, 0, EditorData.GetConstellationEditorConfig());
            newNodeView.SetPosition(ScrollPosition.x + (windowSizeX * 0.5f), ScrollPosition.y + (windowSizeY * 0.5f));
            newNodeView.LockNodePosition();
            SetNodeToFirst(newNodeView);
            editorEvent(ConstellationEditorEvents.EditorEventType.NodeAdded, (string)nodeData.Guid);
        }

        public NodeData[] GetSelectionCopy()
        {
            List<NodeData> selectedNodesData = new List<NodeData>();
            foreach (var nodeView in SelectedNodes)
            {
                var newNode = new NodeData(nodeView.NodeData);
                newNode.Guid = new System.Guid().ToString();
                selectedNodesData.Add(newNode);
            }

            return selectedNodesData.ToArray();
        }

        public void AddNodes(NodeData[] NodesToAdd)
        {
            foreach (var node in NodesToAdd)
            {
                Nodes.Add(new NodeView(node));
            }
        }

        public void RemoveNode(NodeData node, ConstellationEditorEvents.EditorEvents callback)
        {
            foreach (var nodeView in Nodes)
            {
                if (nodeView.NodeData.Guid == node.Guid)
                {
                    callback(ConstellationEditorEvents.EditorEventType.AddToUndo, "Delete node");
                    ReleaseFocus();
                    SelectedNodes.Remove(nodeView);
                    Nodes.Remove(nodeView);
                    callback(ConstellationEditorEvents.EditorEventType.NodeDeleted, node.Guid);
                    constellationRules.RemoveNode(nodeView.NodeData, ConstellationScript.script);
                    return;
                }
            }
        }

        public void UpdateSize(float _windowSizeX, float _windowSizeY)
        {
            windowSizeX = _windowSizeX;
            windowSizeY = _windowSizeY;
        }

        public void Draw(ConstellationEditorEvents.RequestRepaint requestRepaint, ConstellationEditorEvents.EditorEvents callback, ConstellationEditorStyles constellationEditorStyles, out Vector2 windowSize, out Vector2 scrollPosition)
        {
            mouseButtonDown = false;
            //scroll bar
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, GUILayout.Width(windowSizeX), GUILayout.Height(windowSizeY));
            GUILayoutOption[] options = { GUILayout.Width(editorScrollSize.x), GUILayout.Height(editorScrollSize.y) };
            editorScrollSize = new Vector2(farNodeX + 400, farNodeY + 400);
            windowSize = editorScrollSize;
            scrollPosition = ScrollPosition;
            EditorGUILayout.LabelField("", options);
            var backgroundTint = Color.white;
            if (ConstellationScript.IsInstance && ConstellationScript.IsDifferentThanSource)
                backgroundTint = Color.yellow;
            background.DrawBackgroundGrid(windowSizeX, windowSizeY, ScrollPosition.x, ScrollPosition.y, backgroundTint);
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
                mouseButtonDown = true;
            }

            switch (currentEventScope)
            {
                case EventsScope.Generic:
                    UpdateGenericEvents(requestRepaint, callback, e);
                    break;
                case EventsScope.Resizing:
                    UpdateResizeEvents(requestRepaint, callback, e);
                    break;
                case EventsScope.Dragging:
                    UpdateDragEvents(requestRepaint, callback, e);
                    break;
                case EventsScope.EditingAttributes:
                    break;
                case EventsScope.Selecting:
                    UpdateSelectEvent(requestRepaint, constellationEditorStyles);
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
            Links.DrawLinks(requestRepaint,
            callback,
            constellationEditorStyles);
            DrawDescriptions(e);
            EditorGUILayout.EndScrollView();
            if (Event.current.button == 2)
            {
                ScrollPosition -= Event.current.delta * 0.5f;
                requestRepaint();
            }
            var script = ConstellationScript.script;
            if (script.Nodes != null)
                script.Nodes = script.Nodes.OrderBy(x => x.YPosition).ToList();
            if (script.Links != null)
                script.Links = script.Links.OrderBy(x => x.outputPositionY).ToList();
        }

        private void DrawNodes(Event e)
        {
            if (e.type != EventType.MouseDown && e.button != 0)
            {
                farNodeX =
                    0;
                farNodeY = 0;
            }
            //Read in reverse so first element in in front;
            for (int i = Nodes.Count - 1; i >= 0; i--)
            {
                Nodes[i].DrawNode(e, EditorData.GetConstellationEditorConfig(), LockFocus, ReleaseFocus, focusedNode);
                farNodeX = Mathf.Max(Nodes[i].GetPositionX(), farNodeX);
                farNodeY = Mathf.Max(Nodes[i].GetPositionY(), farNodeY);
            }

        }

        public void GetFarNode(out float _farNodeX, out float _farNodeY)
        {
            _farNodeX = farNodeX;
            _farNodeY = farNodeY;
        }

        void LockFocus(string nodeGUID)
        {
            focusedNode = nodeGUID;
        }

        void ReleaseFocus()
        {
            focusedNode = "";
        }

        private void UpdateSelectEvent(ConstellationEditorEvents.RequestRepaint requestRepaint, ConstellationEditorStyles constellationEditorStyles)
        {
            var sizeX = Event.current.mousePosition.x - mouseClickStartPosition.x;
            var sizeY = Event.current.mousePosition.y - mouseClickStartPosition.y;
            var SelectionSize = FixNegativeSize(new Rect(mouseClickStartPosition.x, mouseClickStartPosition.y, sizeX, sizeY));
            GUI.Box(SelectionSize, "", constellationEditorStyles.SelectionAreaStyle);
            if(Event.current.type == EventType.MouseUp)
            {
                if(!Event.current.control) 
                    ClearSelectedNodes();

                foreach(var node in Nodes)
                {
                    if (SelectionSize.Contains(new Vector2(node.GetPositionX() + (node.GetSizeX() * 0.5f), node.GetPositionY() + (node.GetSizeY() * 0.5f)))) {
                        node.SelectedNode();
                        SelectedNodes.Add(node);
                    }
                }
            }
            requestRepaint();
        }

        private Rect FixNegativeSize(Rect rectOld)
        {
            var rect = new Rect(rectOld);

            if (rect.width < 0)
            {
                rect.x += rect.width;
                rect.width = Mathf.Abs(rect.width);
            }

            if (rect.height < 0)
            {
                rect.y += rect.height;
                rect.height = Mathf.Abs(rect.height);
            }

            return rect;
        }

        private void UpdateResizeEvents(ConstellationEditorEvents.RequestRepaint requestRepaint, ConstellationEditorEvents.EditorEvents editorEvents, Event e)
        {
            editorEvents(ConstellationEditorEvents.EditorEventType.NodeResized, "");
            for (var i = 0; i < SelectedNodes.Count; i++)
            {
                SelectedNodes[i].UpdateNodeSize((e.mousePosition.x - mouseClickStartPosition.x) + SelectedNodes[i].GetPreviousNodeSizeX(), (e.mousePosition.y - mouseClickStartPosition.y) + SelectedNodes[i].GetPreviousNodeSizeY(), EditorData.GetConstellationEditorConfig());
            }
            requestRepaint();
        }

        private void UpdateDragEvents(ConstellationEditorEvents.RequestRepaint requestRepaint, ConstellationEditorEvents.EditorEvents editorEvents, Event e)
        {
            editorEvents(ConstellationEditorEvents.EditorEventType.NodeMoved, "");
            for (var i = 0; i < SelectedNodes.Count; i++)
            {
                SelectedNodes[i].SetPosition((e.mousePosition.x - mouseClickStartPosition.x) + SelectedNodes[i].GetPreviousNodePositionX(), (e.mousePosition.y - mouseClickStartPosition.y) + SelectedNodes[i].GetPreviousNodePositionY());
            }
            requestRepaint();
        }

        public void Update(Constellation.Constellation constellation, ConstellationEditorEvents.EditorEvents editorEvents)
        {
            foreach (var node in constellation.GetNodes())
            {
                foreach (var nodeData in Nodes)
                {
                    if (node.Guid == nodeData.NodeData.Guid)
                    {
                        var changedParameters = nodeData.IsParameterValueChanged();
                        if (changedParameters.Length == 0)
                        {
                            for (var i = 0; i < node.GetParameters().Length; i++)
                            {
                                nodeData.NodeData.ParametersData[i].Value.Set(node.GetParameters()[i].Value.GetString());
                            }
                            node.XPosition = nodeData.GetPositionX();
                            node.YPosition = nodeData.GetPositionY();
                            node.XSize = nodeData.GetSizeX();
                            node.YSize = nodeData.GetSizeY();
                        }
                        else
                        {
                            for (var i = 0; i < node.GetParameters().Length; i++)
                            {
                                if (changedParameters.Contains(i)) {
                                    if (ConstellationScript.IsInstance)
                                        ConstellationScript.IsDifferentThanSource = true;
                                    node.GetParameters()[i].Value.Set(nodeData.NodeData.ParametersData[i].Value);
                                    node.NodeType.Receive(nodeData.NodeData.ParametersData[i].Value, new Constellation.Input("0000-0000-0000-0000", Parameter.ParameterInputID + i, true, "editor", "none"));
                                    if (node.NodeType is IParameterUpdate)
                                    {
                                        IParameterUpdate needAttributeUpdate = node.NodeType as IParameterUpdate;
                                        needAttributeUpdate.OnParametersUpdate();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DrawDescriptions(Event e)
        {
            var predictedCharacterSize = 8;
            var minimumSize = 10;
            var spacing = 10;
            for (var i = 0; i < Nodes.Count; i++)
            {
                for (var j = 0; j < Nodes[i].GetInputs().Length; j++)
                {
                    var inputRect = Nodes[i].GetInputRect(j, EditorData.GetConstellationEditorConfig());
                    if (inputRect.Contains(e.mousePosition))
                    {
                        var size = Nodes[i].GetInputs()[j].Description.Length * predictedCharacterSize + minimumSize;
                        GUI.Box(new Rect(e.mousePosition.x - size - spacing, e.mousePosition.y, size, 20), Nodes[i].GetInputs()[j].Description);
                    }
                }

                for (var j = 0; j < Nodes[i].GetOutputs().Length; j++)
                {
                    var outputRect = Nodes[i].GetOuptputRect(j, EditorData.GetConstellationEditorConfig());
                    if (outputRect.Contains(e.mousePosition))
                    {
                        var size = Nodes[i].GetOutputs()[j].Description.Length * predictedCharacterSize + minimumSize;
                        GUI.Box(new Rect(e.mousePosition.x + spacing, e.mousePosition.y, size, 20), Nodes[i].GetOutputs()[j].Description);
                    }
                }
            }
        }

        private void UpdateGenericEvents(ConstellationEditorEvents.RequestRepaint requestRepaint, ConstellationEditorEvents.EditorEvents editorEvents, Event e)
        {
            if (e.type == EventType.Repaint)
                mousePosition = e.mousePosition;

            if (Event.current.keyCode == KeyCode.Delete)
            {
                for (var i = 0; i < SelectedNodes.Count; i++)
                {
                    RemoveNode(SelectedNodes[SelectedNodes.Count - 1].NodeData, editorEvents);
                }
            }

            for (var i = 0; i < Nodes.Count; i++)
            {
                var nodeRect = Nodes[i].GetNodeRect(out float positionOffsetX, out float positionOffsetY);
                var deleteRect = Nodes[i].GetDeleteRect(EditorData.GetConstellationEditorConfig());
                var questionRect = Nodes[i].GetQuestionRect(EditorData.GetConstellationEditorConfig());
                var resizeRect = Nodes[i].GetResizeRect(EditorData.GetConstellationEditorConfig());

                if (nodeRect.Contains(mousePosition))
                {
                    if (mousePressed)
                    {
                        requestRepaint();
                        if (e.control || SelectedNodes.Count == 0)
                        {
                            SelectedNodes.Add(Nodes[i]);
                            Nodes[i].SelectedNode();
                        }
                        else if (SelectedNodes.Count <= 1)
                        {
                            foreach (var selectedNodes in SelectedNodes)
                            {
                                selectedNodes.UnselectNode();
                            }
                            SelectedNodes.Clear();
                            SelectedNodes.Add(Nodes[i]);
                            Nodes[i].SelectedNode();
                        }

                        for (var j = 0; j < Nodes[i].GetInputs().Length; j++)
                        {
                            var inputRect = Nodes[i].GetInputRect(j, EditorData.GetConstellationEditorConfig());
                            if (inputRect.Contains(mousePosition))
                                Links.AddLinkFromInput(Nodes[i].GetInputs()[j],
                                    (ConstellationEditorEvents.EditorEventType editorEventType, string message) =>
                                    {
                                        editorEvents(editorEventType, message);
                                        if (editorEventType == ConstellationEditorEvents.EditorEventType.LinkAdded)
                                        {
                                            UpdateGenericNodeByLinkGUID(message);
                                        }
                                    });
                        }

                        for (var j = 0; j < Nodes[i].GetOutputs().Length; j++)
                        {
                            var outputRect = Nodes[i].GetOuptputRect(j, EditorData.GetConstellationEditorConfig());
                            if (outputRect.Contains(mousePosition))
                                Links.AddLinkFromOutput(Nodes[i].GetOutputs()[j],
                                    (ConstellationEditorEvents.EditorEventType editorEventType, string message) =>
                                    {
                                        editorEvents(editorEventType, message);
                                        if (editorEventType == ConstellationEditorEvents.EditorEventType.LinkAdded)
                                            UpdateGenericNodeByLinkGUID(message);
                                    });
                        }

                        if (deleteRect.Contains(mousePosition) && mouseButtonDown)
                        {
                            RemoveNode(Nodes[i].NodeData, editorEvents);
                            return;
                        }

                        if (questionRect.Contains(mousePosition) && mouseButtonDown)
                        {
                            editorEvents(ConstellationEditorEvents.EditorEventType.HelpClicked, Nodes[i].GetName());
                            return;
                        }

                        for (var j = 0; j < Nodes[i].GetAttributeDatas().Length; j++)
                        {
                            var attributeRect = Nodes[i].GetParameterRect(j, EditorData.GetConstellationEditorConfig());
                            if (attributeRect.Contains(mousePosition))
                            {
                                editorEvents(ConstellationEditorEvents.EditorEventType.AddToUndo, "Attribute edited");
                                currentEventScope = EventsScope.EditingAttributes;
                                return;
                            }
                        }

                        if (mouseButtonDown)
                        {
                            if (resizeRect.Contains(mousePosition))
                            {
                                currentEventScope = EventsScope.Resizing;
                                return;
                            }
                            editorEvents(ConstellationEditorEvents.EditorEventType.AddToUndo, "Node moved");
                            SetNodeToFirst(Nodes[i]);
                            currentEventScope = EventsScope.Dragging;
                            return;
                        }
                    }
                } else if(mousePressed && e.mousePosition.x != mouseClickStartPosition.x && e.mousePosition.y != mouseClickStartPosition.y)
                {
                    currentEventScope = EventsScope.Selecting;
                } 
            }

            if (e.MouseUp() && e.button != 2)
            {
                foreach (var node in SelectedNodes)
                {
                    node.UnselectNode();
                }
                SelectedNodes.Clear();
            }
        }

        private void UpdateGenericNodeByLinkGUID(string guid)
        {
            constellationRules.UpdateGenericNodeByLinkGUID(ConstellationScript.script, NodeFactory, guid);
        }

        private void SetNodeToFirst(NodeView node)
        {
            Nodes.Remove(node);
            Nodes.Insert(0, node);
        }
    }
}
