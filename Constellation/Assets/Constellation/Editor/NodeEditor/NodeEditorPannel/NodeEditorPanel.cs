//[TODO] AC Split this class into multiples ones. For example background could have it's own class.
using System.Linq;
using Constellation;
using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {
    public class NodeEditorPanel : IVisibleObject {
        Vector2 editorScrollPos;
        Vector2 editorScrollSize;
        private const string editorPath = "Assets/Constellation/Editor/EditorAssets/";
        private string[] nodes;

        private EditorWindow EditorWindow;
        private NodeEditorNodes NodeEditorNodes;
        private IGUI GUI;
        private ConstellationScript constellationScript;
        private NodeConfig nodeConfig;
        private IUndoable undoable;
        private NodeEditorSelection nodeEditorSelection;
        private Vector2 panelSize = Vector2.zero;
        private bool isInstance;
        NodeEditorLinks.LinkAdded OnLinkAdded;
        NodeEditorLinks.LinkRemoved OnLinkRemoved;
        NodeEditorNodes.NodeAdded OnNodeAdded;
        NodeEditorNodes.NodeRemoved OnNodeRemoved;
        NodeEditorNodes.HelpClicked OnHelpClicked;

        public delegate void ApplyInstanceChanges ();
        ApplyInstanceChanges OnApplyInstanceChanges;
        private NodeEditorBackground Background;
        public NodeEditorLinks NodeEditorLinks;
        private bool isSetupRequested = false;

        public NodeEditorPanel (IGUI _gui,
            EditorWindow _editorWindow,
            ConstellationScript _script,
            IUndoable _undoable,
            ClipBoard _editorClipBoard,
            float positionX,
            float positionY,
            NodeEditorLinks.LinkAdded linkAdded,
            NodeEditorLinks.LinkRemoved onLinkRemoved,
            NodeEditorNodes.NodeAdded nodeAdded,
            NodeEditorNodes.NodeRemoved nodeRemoved,
            NodeEditorNodes.HelpClicked onHelpClicked,
            ApplyInstanceChanges applyInstanceChanges) {
            constellationScript = _script;
            undoable = _undoable;
            GUI = _gui;
            EditorWindow = _editorWindow;
            editorScrollSize = new Vector2 (500, 500);
            var backgroundTexture = AssetDatabase.LoadAssetAtPath (editorPath + "background.png", typeof (Texture2D)) as Texture2D;
            Background = new NodeEditorBackground (GUI, backgroundTexture);

            var allNodes = NodesFactory.GetAllNodes ();
            nodes = new string[allNodes.Length];
            editorScrollPos = new Vector2 (positionX, positionY);

            for (var i = 0; i < allNodes.Length; i++) {
                nodes[i] = allNodes[i];
            }
            OnLinkAdded += linkAdded;
            OnNodeAdded += nodeAdded;
            OnNodeRemoved += nodeRemoved;
            OnApplyInstanceChanges += applyInstanceChanges;
            OnHelpClicked += onHelpClicked;
            OnLinkRemoved += onLinkRemoved;
            nodeEditorSelection = new NodeEditorSelection (GUI, _editorClipBoard);
            RequestSetup ();
        }

        public void Initialize () {
            NodeEditorLinks = new NodeEditorLinks (constellationScript, constellationScript.IsInstance, GUI, nodeConfig, AddedLink, RemovedLink, this, undoable);
            NodeEditorNodes = new NodeEditorNodes (EditorWindow, nodeConfig, constellationScript, undoable, nodeEditorSelection, NodeEditorLinks, GUI, this, OnNodeAdded, OnNodeRemoved, OnHelpClicked);
        }

        void SetNodes () {

        }

        void LoadConstellation () {
            if (constellationScript == null)
                throw new ConstellationScriptDataDoesNotExist ();

            if (NodeEditorLinks == null)
                Initialize ();

            if (constellationScript.IsInstance) {
                isInstance = true;
                //Here should check if is different
                constellationScript.IsDifferentThanSource = true;
            }
        }

        public void RequestHelp (string _nodeName) {
            OnHelpClicked (_nodeName);
        }

        public void Update (Constellation.Constellation constellation) {
            foreach (var node in constellation.GetNodes ()) {
                foreach (var nodeData in NodeEditorNodes.GetNodes ()) {
                    if (node.Guid == nodeData.node.Guid) {
                        for (var i = 0; i < node.GetAttributes ().Length; i++) {
                            if (!nodeData.IsAttributeValueChanged ()) {
                                nodeData.GetData ().AttributesData[i].Value.Set (node.GetAttributes () [i].Value.GetString ());
                                if (node.NodeType is IAttributeUpdate) {
                                    IAttributeUpdate needAttributeUpdate = node.NodeType as IAttributeUpdate;
                                    needAttributeUpdate.OnAttributesUpdate ();
                                }
                            } else {
                                if (isInstance)
                                    constellationScript.IsDifferentThanSource = true;
                                node.GetAttributes () [i].Value.Set (nodeData.GetData ().AttributesData[i].Value);
                                node.NodeType.Receive (nodeData.GetData ().AttributesData[i].Value, new Constellation.Input ("0000-0000-0000-0000", 999, true, "editor", "none"));
                            }
                        }
                    }
                }
            }
        }

        public void SelectNodes (NodeData[] _nodes) {
            NodeEditorNodes.SelectNodes (_nodes);
        }

        public void AddNode (string _nodeName, string _namespace) {
            NodeEditorNodes.AddNode (_nodeName, _namespace, panelSize, editorScrollPos);
        }

        private void AddedLink (LinkData link) {
            OnLinkAdded (link);
        }

        private void RemovedLink (LinkData link) {
            OnLinkRemoved (link);
        }

        public NodeEditorSelection GetNodeSelection () {
            return nodeEditorSelection;
        }

        void RequestSetup () {
            isSetupRequested = true;
        }

        void Setup () {
            nodeConfig = new NodeConfig ();
            LoadConstellation ();
            isSetupRequested = false;
        }

        public LinkData[] GetLinks () {
            return NodeEditorLinks.GetLinks ();
        }

        public void RequestRepaint () {
            GUI.RequestRepaint ();
        }

        public float GetCurrentScrollPosX () {
            return editorScrollPos.x;
        }

        public float GetCurrentScrollPosY () {
            return editorScrollPos.y;
        }

        public void DrawNodeEditor (Rect LayoutPosition) {
            if (isSetupRequested)
                Setup ();

            panelSize = new Vector2 (LayoutPosition.width, LayoutPosition.height);
            editorScrollPos = EditorGUILayout.BeginScrollView (editorScrollPos, false, false, GUILayout.Width (LayoutPosition.width), GUILayout.Height (LayoutPosition.height));
            GUILayoutOption[] options = { GUILayout.Width (editorScrollSize.x), GUILayout.Height (editorScrollSize.y) };
            EditorGUILayout.LabelField ("", options);
            var backgroundTint = Color.white;
            if (isInstance && constellationScript.IsDifferentThanSource)
                backgroundTint = Color.yellow;
            Background.DrawBackgroundGrid (LayoutPosition.width, LayoutPosition.height, GetCurrentScrollPosX (), GetCurrentScrollPosY (), backgroundTint);
            NodeEditorNodes.DrawEditorNodes (editorScrollPos);
            NodeEditorLinks.DrawLinks ();
            EditorGUILayout.EndScrollView ();
            if (isInstance)
                DrawInstancePannel ();
            editorScrollSize = new Vector2 (NodeEditorNodes.GetFarNodeX () + 400, NodeEditorNodes.GetFarNodeY () + 400);
            nodeEditorSelection.Draw (NodeEditorNodes.GetNodes ().ToArray (), GetLinks (), editorScrollPos, LayoutPosition);

            if (Event.current.button == 2) {
                editorScrollPos -= Event.current.delta * 0.5f;
                RequestRepaint ();
            }
        }

        private void DrawInstancePannel () {
            if (!constellationScript.IsDifferentThanSource || NodeEditorNodes.IsTutorial ())
                return;

            GUI.SetColor (Color.yellow);
            Event e = Event.current;
            var x = 0;
            var y = 40;
            var width = 100;
            var height = 25;
            if (GUI.DrawButton (new Rect (x, y, width, height), "Apply")) {
                if (isInstance) {
                    constellationScript.IsDifferentThanSource = false;
                }
                OnApplyInstanceChanges ();
            }
            GUI.SetColor (Color.white);
        }

        public bool InView (Rect rect) {
            var scrollX = GetCurrentScrollPosX ();
            var scrollY = GetCurrentScrollPosY ();
            var view = new Rect (scrollX, scrollY, scrollX + GetWidth (), scrollY + GetHeight ());
            return view.Overlaps (rect);
        }

        public float GetWidth () {
            return panelSize.x;
        }

        public float GetHeight () {
            return panelSize.y;
        }

        public Vector2 GetScrollSize () {
            return editorScrollSize;
        }
    }
}