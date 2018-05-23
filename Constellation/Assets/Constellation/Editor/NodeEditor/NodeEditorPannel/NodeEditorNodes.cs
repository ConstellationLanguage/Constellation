using System.Collections.Generic;
using System.Linq;
using Constellation;
using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {

    public class NodeEditorNodes {
        private bool isDraggingWindow;
        private float farNodeX;
        private float farNodeY;
        private EditorWindow editorWindow;
        public List<NodeView> Nodes;
        private NodeConfig nodeConfig;
        private bool isInstance;
        private ConstellationScript constellationScript;
        private NodesFactory nodesFactory;
        private IUndoable undoable;
        private NodeEditorSelection nodeEditorSelection;
        private ILinkEditor linkEditor;
        private IGUI GUI;
        private IVisibleObject visibleObject;
        public delegate void NodeAdded (NodeData node);
        NodeAdded OnNodeAdded;
        public delegate void NodeRemoved (NodeData node);
        NodeRemoved OnNodeRemoved;
        public delegate void HelpClicked (string NodeName);
        HelpClicked OnHelpClicked;
        private bool isTutorial = false;

        public NodeEditorNodes (EditorWindow _editorWindow,
            NodeConfig _nodeConfig,
            ConstellationScript _constellationScript,
            IUndoable _undoable,
            NodeEditorSelection _nodeEditorSelection,
            ILinkEditor _linkEditor,
            IGUI _gui,
            IVisibleObject _visibleObject,
            NodeAdded _nodeAdded,
            NodeRemoved _nodeRemoved,
            HelpClicked _helpClicked) {

            linkEditor = _linkEditor;
            editorWindow = _editorWindow;
            Nodes = new List<NodeView> ();
            nodeConfig = _nodeConfig;
            constellationScript = _constellationScript;
            isInstance = constellationScript.IsInstance;
            nodesFactory = new NodesFactory ();
            undoable = _undoable;
            nodeEditorSelection = _nodeEditorSelection;
            GUI = _gui;
            visibleObject = _visibleObject;
            OnNodeAdded += _nodeAdded;
            OnNodeRemoved += _nodeRemoved;
            OnHelpClicked += _helpClicked;
            SetNodes();
        }

        public float GetFarNodeX () {
            return farNodeX;
        }

        public float GetFarNodeY () {
            return farNodeY;
        }

        private void SetNodes () {
            foreach (NodeData nodeData in constellationScript.GetNodes ()) {
                if (nodeData.Name == "Tutorial") {
                    isTutorial = true;
                }
                Nodes.Add (new NodeView (nodeData, visibleObject, nodeConfig, constellationScript, linkEditor));
            }
        }

        public void DrawEditorNodes (Vector2 editorScrollPos) {

            farNodeX = 0;
            farNodeY = 0;
            editorWindow.BeginWindows ();
            var i = 0;
            if (Nodes == null)
                return;

            if (Event.current.button == 2) {
                editorScrollPos -= Event.current.delta * 0.5f;
                GUI.RequestRepaint ();
            }

            foreach (NodeView node in Nodes) {
                if (node == null)
                    return;

                node.DrawWindow (i, DrawNodeWindow, false);
                i++;
                farNodeX = Mathf.Max (node.GetRect ().x, farNodeX);
                farNodeY = Mathf.Max (node.GetRect ().y, farNodeY);
            }
            editorWindow.EndWindows ();
        }

        public NodeView[] GetNodes () {
            return Nodes.ToArray ();
        }
        public void DrawNodeWindow (int id) {
            if (id < Nodes.Count) {
                if (Nodes[id].NodeExist ()) {
                    Nodes[id].DrawContent (HelpRequested);
                } else {
                    OnNodeRemoved (Nodes[id].node);
                    Nodes.Remove (Nodes[id]);
                    undoable.AddAction ();
                }
            }

            if (Event.current.delta == Vector2.zero && isDraggingWindow && Event.current.isMouse) {
                undoable.AddAction ();
                isDraggingWindow = false;

                if (isInstance)
                    constellationScript.IsDifferentThanSource = true;
            } else if (Event.current.button == 0) {
                isDraggingWindow = true;
            }

            var script = constellationScript.script;

            if (script.Nodes != null)
                script.Nodes = script.Nodes.OrderBy (x => x.YPosition).ToList ();
            if (script.Links != null)
                script.Links = script.Links.OrderBy (x => x.outputPositionY).ToList ();

            if (Event.current.button == 0) {
                GUI.DragWindow ();
            }
            if (constellationScript != null)
                EditorUtility.SetDirty (constellationScript);
        }

        public NodeData AddNode (string _nodeName, string _namespace, Vector2 panelSize, Vector2 editorScrollPos) {

            if (isInstance)
                constellationScript.IsDifferentThanSource = true;

            var newNode = constellationScript.AddNode (nodesFactory.GetNode (_nodeName, _namespace));
            newNode.XPosition = editorScrollPos.x + (panelSize.x * 0.5f);
            newNode.YPosition = editorScrollPos.y + (panelSize.y * 0.5f);
            var newNodeWindow = new NodeView (newNode, visibleObject, nodeConfig, constellationScript, linkEditor);
            Nodes.Add (newNodeWindow);
            undoable.AddAction ();
            OnNodeAdded (newNode);
            nodeEditorSelection.UnselectAll ();
            return newNode;
        }

        void HelpRequested (string nodeName) {
            OnHelpClicked (nodeName);
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

        public bool IsTutorial()
        {
            return isTutorial;
        }

    }
}