//C# Example
using Constellation;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {
    public class NodeEditorPanel {
        Vector2 editorScrollPos;
        Vector2 editorScrollSize;
        private Texture2D Background;
        private const string editorPath = "Assets/Constellation/Editor/EditorAssets/";
        private string[] nodes;
        public List<NodeView> Nodes;
        public LinkView LinksView;
        private EditorWindow EditorWindow;
        private IGUI GUI;
        private IConstellationScript constellationScript;
        private InputData selectedInput;
        private OutputData selectedOutput;
        private NodeConfig nodeConfig;
        private float farNodeX;
        private float farNodeY;
        private IUndoable undoable;
        private bool isDraggingWindow;
        private NodeEditorSelection nodeEditorSelection;
        private Vector2 panelSize = Vector2.zero;
        public delegate void LinkAdded (LinkData link);
        LinkAdded OnLinkAdded;
        public delegate void NodeAdded (NodeData node);
        NodeAdded OnNodeAdded;
        public delegate void NodeRemoved (NodeData node);
        NodeRemoved OnNodeRemoved;
        private NodesFactory nodesFactory;

        public NodeEditorPanel (IGUI _gui,
            EditorWindow _editorWindow,
            IConstellationScript _script,
            IUndoable _undoable,
            ClipBoard _editorClipBoard,
            float positionX,
            float positionY,
            LinkAdded linkAdded,
            NodeAdded nodeAdded,
            NodeRemoved nodeRemoved) {
            nodesFactory = new NodesFactory ();
            constellationScript = _script;
            undoable = _undoable;
            Nodes = new List<NodeView> ();
            GUI = _gui;
            EditorWindow = _editorWindow;
            editorScrollSize = new Vector2 (500, 500);
            Background = AssetDatabase.LoadAssetAtPath (editorPath + "background.png", typeof (Texture2D)) as Texture2D;
            var allNodes = NodesFactory.GetAllNodes ();
            nodes = new string[allNodes.Length];
            editorScrollPos = new Vector2 (positionX, positionY);
            for (var i = 0; i < allNodes.Length; i++) {
                nodes[i] = allNodes[i];
            }
            OnLinkAdded += linkAdded;
            OnNodeAdded += nodeAdded;
            OnNodeRemoved += nodeRemoved;
            nodeEditorSelection = new NodeEditorSelection (GUI, _editorClipBoard);
        }

        void LoadConstellation () {
            foreach (NodeData nodeData in constellationScript.GetNodes ()) {
                Nodes.Add (new NodeView (nodeData, this, nodeConfig, constellationScript));
            }
        }

        public void Update (Constellation.Constellation constellation) {
            foreach (var node in constellation.GetNodes ()) {
                foreach (var nodeData in Nodes) {
                    if (node.Guid == nodeData.node.Guid) {
                        for (var i = 0; i < node.GetAttributes ().Length; i++) {
                            if (!nodeData.IsAttributeValueChanged ())
                                nodeData.GetData ().AttributesData[i].Value.Set (node.GetAttributes () [i].Value.GetString ());
                            else {
                                node.GetAttributes () [i].Value.Set (nodeData.GetData ().AttributesData[i].Value);
                                node.NodeType.Receive (nodeData.GetData ().AttributesData[i].Value, new Constellation.Input ("0000-0000-0000-0000", 999, true, "editor", "none"));
                            }
                        }
                    }
                }
            }
        }

        public NodeEditorSelection GetNodeSelection () {
            return nodeEditorSelection;
        }

        public void SelectNodes (NodeData[] _nodes) {
            if (_nodes == null)
                return;
            nodeEditorSelection.UnselectAll ();
            foreach (var nodeData in _nodes) {
                foreach (var nodeView in Nodes) {
                    if (nodeData.Guid == nodeView.GetData ().Guid) {
                        nodeEditorSelection.SelectNode (nodeView);
                    }
                }
            }
        }

        void Setup () {
            nodeConfig = new NodeConfig ();
            LoadConstellation ();
            LinksView = new LinkView (GUI, this, constellationScript, nodeConfig);
        }

        void DrawEditorNodes () {
            if (nodeConfig == null) {
                Setup ();
            }

            farNodeX = 0;
            farNodeY = 0;
            EditorWindow.BeginWindows ();
            var i = 0;
            if (Nodes == null)
                return;
            
            if (Event.current.button == 2) {
                editorScrollPos -= Event.current.delta * 0.5f;
                RequestRepaint();
            }
            
            foreach (NodeView node in Nodes) {
                if (node == null)
                    return;
                
                node.DrawWindow (i, DrawNodeWindow, false);
                i++;
                farNodeX = Mathf.Max (node.GetRect ().x, farNodeX);
                farNodeY = Mathf.Max (node.GetRect ().y, farNodeY);
            }
            EditorWindow.EndWindows ();
        }

        public NodeData AddNode (string _nodeName, string _namespace) {
            var newNode = constellationScript.AddNode (nodesFactory.GetNode (_nodeName, _namespace));
            newNode.XPosition = editorScrollPos.x + (panelSize.x * 0.5f);
            newNode.YPosition = editorScrollPos.y + (panelSize.y * 0.5f);
            var newNodeWindow = new NodeView (newNode, this, nodeConfig, constellationScript);
            Nodes.Add (newNodeWindow);
            undoable.AddAction ();
            OnNodeAdded (newNode);
            return newNode;
        }

        void DrawNodeWindow (int id) {
            if (id < Nodes.Count) {
                if (Nodes[id].NodeExist ()) {
                    Nodes[id].DrawContent();
                } else {
                    OnNodeRemoved (Nodes[id].node);
                    Nodes.Remove (Nodes[id]);
                    undoable.AddAction ();
                }
            }
            
            if (Event.current.delta == Vector2.zero && isDraggingWindow && Event.current.isMouse) {
                undoable.AddAction ();
                isDraggingWindow = false;
            } else if (Event.current.button == 0) {
                isDraggingWindow = true;
            }
            
            var script = constellationScript.GetData();
            script.Nodes = script.Nodes.OrderBy (x => x.YPosition).ToList ();
            script.Links = script.Links.OrderBy (x => x.outputPositionY).ToList ();
            if(Event.current.button == 0)
                GUI.DragWindow ();
            EditorUtility.SetDirty (constellationScript.GetScriptObject() as UnityEngine.Object);
        }

        private void DrawIncompleteLink () {
            if (selectedInput != null || selectedOutput != null) {
                var e = Event.current;
                if (selectedInput != null) {
                    LinksView.DrawNodeCurve (new Rect (e.mousePosition.x, e.mousePosition.y, 0, 0), LinksView.InputPosition (selectedInput));
                    GUI.RequestRepaint ();
                } else if (selectedOutput != null) {
                    LinksView.DrawNodeCurve (LinksView.OutputPosition (selectedOutput), new Rect (e.mousePosition.x, e.mousePosition.y, 0, 0));
                    GUI.RequestRepaint ();
                }

                if (e.button == 1) {
                    selectedInput = null;
                    selectedOutput = null;
                }
            }
        }

        public void RequestRepaint () {
            GUI.RequestRepaint ();
        }

        public void AddLinkFromOutput (OutputData _output) {
            if (selectedInput != null)
                CreateLink (selectedInput, _output);
            else if (selectedOutput == null)
                selectedOutput = _output;
        }

        public void AddLinkFromInput (InputData _input) {
            if (selectedOutput != null)
                CreateLink (_input, selectedOutput);
            else if (selectedInput == null)
                selectedInput = _input;
        }

        private void CreateLink (InputData _input, OutputData _output) {
            selectedInput = null;
            selectedOutput = null;
            var newLink = new LinkData (_input, _output);
            if (constellationScript.IsLinkValid (newLink)) {
                constellationScript.AddLink (newLink);
                OnLinkAdded (newLink);
                undoable.AddAction ();
                GUI.RequestRepaint ();
            }
        }

        public float GetCurrentScrollPosX () {
            return editorScrollPos.x;
        }

        public float GetCurrentScrollPosY () {
            return editorScrollPos.y;
        }

        public void DrawNodeEditor (float _width, float _height) {
            panelSize = new Vector2 (_width, _height);
            
            editorScrollPos = EditorGUILayout.BeginScrollView (editorScrollPos, false, false, GUILayout.Width (_width), GUILayout.Height (_height));
            GUILayoutOption[] options = { GUILayout.Width (editorScrollSize.x), GUILayout.Height (editorScrollSize.y) };
            EditorGUILayout.LabelField ("", options); 
            
            DrawBackgroundGrid (_width, _height);
            DrawEditorNodes ();
            LinksView.DrawLinks ();
            DrawIncompleteLink ();

            EditorGUILayout.EndScrollView ();
            editorScrollSize = new Vector2 (farNodeX + 400, farNodeY + 400);
            nodeEditorSelection.Draw (Nodes.ToArray (), LinksView.GetLinks (), editorScrollPos);
        }

        private void DrawBackgroundGrid (float _width, float _height) {
            if (Background != null) {
                //Background location based of current location allowing unlimited background
                //How many background are needed to fill the background
                var xCount = Mathf.Round (_width / Background.width) + 2;
                var yCount = Mathf.Round (_height / Background.height) + 2;
                //Current scroll offset for background
                var xOffset = Mathf.Round (GetCurrentScrollPosX () / Background.width) - 1;
                var yOffset = Mathf.Round (GetCurrentScrollPosY () / Background.height) - 1;
                var texRect = new Rect (0, 0, Background.width, Background.height);

                for (var i = xOffset; i < xOffset + xCount; i++) {
                    for (var j = yOffset; j < yOffset + yCount; j++) {
                        texRect.x = i * Background.width;
                        texRect.y = j * Background.height;
                        GUI.DrawTexture (texRect, Background);
                    }
                }
            }
        }

        public bool InView (Rect rect) {
            var scrollX = GetCurrentScrollPosX ();
            var scrollY = GetCurrentScrollPosY ();
            var view = new Rect (scrollX, scrollY, scrollX + GetWidth (), scrollY + GetHeight ());
            return view.Overlaps(rect);
        }

        public float GetWidth () {
            return panelSize.x;
        }

        public float GetHeight () {
            return panelSize.y;
        }
    }
}