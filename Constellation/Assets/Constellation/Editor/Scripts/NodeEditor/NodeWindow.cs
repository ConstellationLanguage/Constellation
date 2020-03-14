using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Constellation;
using System.Linq;

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
        private enum EventsScope { Generic, Resizing, Dragging, EditingAttributes };
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
            NodeFactory = new NodesFactory(ConstellationScript?.ScriptAssembly?.GetAllScriptData());

            foreach (var node in _constellationScript.GetNodes())
            {
                DisplayNode(node);
            }
        }

        public void DisplayNode(NodeData node)
        {
            var nodeView = new NodeView(node);
            if (node.SizeX == 0 || node.SizeY == 0)
                nodeView.UpdateNodeSize(0, 0);
            Nodes.Add(nodeView);
        }

        public void AddNode(string nodeName, string nodeNamespace, ConstellationEditorEvents.EditorEvents callback)
        {
            var newNode = NodeFactory.GetNode(nodeName, nodeNamespace);
            var nodeData = new NodeData(newNode);
            nodeData = ConstellationScript.AddNode(nodeData);
            nodeData.XPosition = 0;
            nodeData.YPosition = 0;
            var newNodeView = new NodeView(nodeData);
            Nodes.Add(newNodeView);
            newNodeView.UpdateNodeSize(0, 0);
            newNodeView.SetPosition(ScrollPosition.x + (windowSizeX * 0.5f), ScrollPosition.y + (windowSizeY * 0.5f));
            newNodeView.LockNodePosition();
            callback(ConstellationEditorEvents.EditorEventType.NodeAdded, nodeData.Guid);
            SetNodeToFirst(newNodeView);
        }

        public void RemoveNode(NodeData node, ConstellationEditorEvents.EditorEvents callback)
        {
            foreach (var nodeView in Nodes)
            {
                if (nodeView.NodeData.Guid == node.Guid)
                {
                    callback(ConstellationEditorEvents.EditorEventType.NodeDeleted, node.Guid);
                    SelectedNodes.Remove(nodeView);
                    Nodes.Remove(nodeView);
                    ConstellationScript.RemoveNode(nodeView.NodeData);
                    return;
                }
            }
        }

        public void UpdateSize(float _windowSizeX, float _windowSizeY)
        {
            windowSizeX = _windowSizeX;
            windowSizeY = _windowSizeY;
        }

        public void Draw(ConstellationEditorEvents.RequestRepaint requestRepaint, ConstellationEditorEvents.EditorEvents callback)
        {
            mouseButtonDown = false;
            //scroll bar
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, GUILayout.Width(windowSizeX), GUILayout.Height(windowSizeY));
            GUILayoutOption[] options = { GUILayout.Width(editorScrollSize.x), GUILayout.Height(editorScrollSize.y) };
            editorScrollSize = new Vector2(farNodeX + 400, farNodeY + 400);
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
            Links.DrawLinks(requestRepaint, callback);
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
            //Read in reverse so first element in in front;
            for (int i = Nodes.Count - 1; i >= 0; i--)
            {
                Nodes[i].DrawNode(e);
                farNodeX = Mathf.Max(Nodes[i].GetPositionX(), farNodeX);
                farNodeY = Mathf.Max(Nodes[i].GetPositionY(), farNodeY);
            }
        }

        private void UpdateResizeEvents(ConstellationEditorEvents.RequestRepaint requestRepaint, ConstellationEditorEvents.EditorEvents editorEvents, Event e)
        {
            editorEvents(ConstellationEditorEvents.EditorEventType.NodeResized, "");
            for (var i = 0; i < SelectedNodes.Count; i++)
            {
                SelectedNodes[i].UpdateNodeSize((e.mousePosition.x - mouseClickStartPosition.x) + SelectedNodes[i].GetPreviousNodeSizeX(), (e.mousePosition.y - mouseClickStartPosition.y) + SelectedNodes[i].GetPreviousNodeSizeY());
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
                        if (!nodeData.IsAttributeValueChanged())
                        {
                            for (var i = 0; i < node.GetAttributes().Length; i++)
                            {
                                nodeData.NodeData.AttributesData[i].Value.Set(node.GetAttributes()[i].Value.GetString());
                            }
                        }
                        else
                        {
                            for (var i = 0; i < node.GetAttributes().Length; i++)
                            {
                                if (ConstellationScript.IsInstance)
                                    ConstellationScript.IsDifferentThanSource = true;
                                node.GetAttributes()[i].Value.Set(nodeData.NodeData.AttributesData[i].Value);
                                node.NodeType.Receive(nodeData.NodeData.AttributesData[i].Value, new Constellation.Input("0000-0000-0000-0000", 999, true, "editor", "none"));
                                if (node.NodeType is IAttributeUpdate)
                                {
                                    IAttributeUpdate needAttributeUpdate = node.NodeType as IAttributeUpdate;
                                    needAttributeUpdate.OnAttributesUpdate();
                                }
                            }
                        }

                    }
                }
            }
        }

        private void DrawDescriptions(Event e)
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                for (var j = 0; j < Nodes[i].GetInputs().Length; j++)
                {
                    var inputRect = Nodes[i].GetInputRect(j);
                    if (inputRect.Contains(e.mousePosition))
                        GUI.Box(new Rect(e.mousePosition.x - 110, e.mousePosition.y, 100, 20), Nodes[i].GetInputs()[j].Description);
                }

                for (var j = 0; j < Nodes[i].GetOutputs().Length; j++)
                {
                    var outputRect = Nodes[i].GetOuptputRect(j);
                    if (outputRect.Contains(e.mousePosition))
                        GUI.Box(new Rect(e.mousePosition.x + 10, e.mousePosition.y, 100, 20), Nodes[i].GetOutputs()[j].Description);
                }
            }
        }

        private void UpdateGenericEvents(ConstellationEditorEvents.RequestRepaint requestRepaint, ConstellationEditorEvents.EditorEvents editorEvents, Event e)
        {
            for (var i = 0; i < Nodes.Count; i++)
            {
                var nodeRect = Nodes[i].GetNodeRect(out float positionOffsetX, out float positionOffsetY);
                var deleteRect = Nodes[i].GetDeleteRect();
                var questionRect = Nodes[i].GetQuestionRect();
                var resizeRect = Nodes[i].GetResizeRect();

                if (nodeRect.Contains(e.mousePosition))
                {
                    if (mousePressed)
                    {
                        requestRepaint();
                        if (e.control)
                        {
                            SelectedNodes.Add(Nodes[i]);
                            SetNodeToFirst(Nodes[i]);
                        }
                        else
                        {
                            SelectedNodes.Clear();
                            SelectedNodes.Add(Nodes[i]);
                        }

                        for (var j = 0; j < Nodes[i].GetInputs().Length; j++)
                        {
                            var inputRect = Nodes[i].GetInputRect(j);
                            if (inputRect.Contains(e.mousePosition))
                                Links.AddLinkFromInput(Nodes[i].GetInputs()[j], editorEvents);
                        }

                        for (var j = 0; j < Nodes[i].GetOutputs().Length; j++)
                        {
                            var outputRect = Nodes[i].GetOuptputRect(j);
                            if (outputRect.Contains(e.mousePosition))
                                Links.AddLinkFromOutput(Nodes[i].GetOutputs()[j], editorEvents);
                        }

                        if (deleteRect.Contains(e.mousePosition) && mouseButtonDown)
                        {
                            RemoveNode(Nodes[i].NodeData, editorEvents);
                            return;
                        }

                        if (questionRect.Contains(e.mousePosition) && mouseButtonDown)
                        {
                            editorEvents(ConstellationEditorEvents.EditorEventType.HelpClicked, Nodes[i].GetName());
                            return;
                        }

                        for (var j = 0; j < Nodes[i].GetAttributeDatas().Length; j++)
                        {
                            var attributeRect = Nodes[i].GetAttributeRect(j);
                            if (attributeRect.Contains(e.mousePosition))
                            {
                                currentEventScope = EventsScope.EditingAttributes;
                                return;
                            }
                        }

                        if (mouseButtonDown)
                        {
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
                }
                else
                {

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
            Nodes.Insert(0, node);
        }
    }
}
