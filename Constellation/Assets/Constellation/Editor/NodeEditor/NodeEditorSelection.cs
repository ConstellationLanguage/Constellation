using Constellation;
using System.Collections.Generic;
using UnityEngine;

namespace ConstellationEditor {

    public class NodeEditorSelection {
        public List<NodeView> SelectedNodes;
        private Vector2 StartMousePosition;
        private Vector2 DragSize;
        private Vector2 SelectionOffset;
        private IGUI GUI;
        private const int offsetX = 50;
        private const int offsetY = 25;
        public NodeEditorSelection (IGUI gui, ClipBoard clipBoard) {
            SelectedNodes = new List<NodeView> ();
            StartMousePosition = Vector2.zero;
            DragSize = Vector2.zero;
            GUI = gui;
        }

        public void DestroySelection () {
            foreach (var node in SelectedNodes) {
                node.DestroyNode();
            }
            UnselectAll();
        }

        public void Draw (NodeView[] nodes, LinkData[] links, Vector2 selectionOffset) {
            var e = Event.current;
            //Mouse position corrected by view offset
            var hoverPosition = e.mousePosition + selectionOffset;
            var Selection = Rect.zero;
            var SelectionDisplay = Rect.zero;

            if (e.keyCode == KeyCode.Delete)
                DestroySelection();

            DragSelection();
            SelectionDisplay = new Rect(StartMousePosition.x, StartMousePosition.y, DragSize.x, DragSize.y);
            Selection = new Rect(StartMousePosition.x + selectionOffset.x, StartMousePosition.y + selectionOffset.y, DragSize.x, DragSize.y);

            //Special handling for single node. This is to make GUI.DragWindow work correctly
            if (SelectedNodes.Count == 1 && DragSize == Vector2.zero) {
                if (e.type != EventType.Used && e.type != EventType.Layout && e.type != EventType.Repaint) {
                    UnselectAll();
                    SelectNode(nodes, hoverPosition);
                    GUI.RequestRepaint();
                }
                return;
            }

            //Deselect all
            if (e.button == 1 && e.isMouse && (e.type == EventType.MouseDown)) {
                UnselectAll();
                GUI.RequestRepaint();
            }

            //Selecting area
            if (e.button == 0 && e.isMouse && (e.type == EventType.MouseDown)) {
                UnselectAll();
                StartMousePosition = e.mousePosition;
                GUI.RequestRepaint();
            } else if (e.type == EventType.MouseDrag && Selection.x != 0 && Selection.y != 0) {
                DragSize = e.mousePosition - StartMousePosition;
                UnselectAll();
                GUI.RequestRepaint();
            } else if (Selection.x != 0 && Selection.y != 0 && Selection.width != 0 && Selection.height != 0) {
                if (e.type == EventType.MouseUp) {
                    SelectNodes(nodes, Selection);
                    StartMousePosition = Vector2.zero;
                    DragSize = Vector2.zero;
                    SelectionDisplay = Rect.zero;
                    GUI.RequestRepaint();
                }
            }

            //Mouse over
            if (DragSize == Vector2.zero && e.type != EventType.Used && e.type != EventType.MouseDrag) {
                if (SelectedNodes.Count <= 1) {
                    UnselectAll();
                    var selection = SelectNode(nodes, hoverPosition);
                    if (selection != null)
                        SelectNode(selection);
                }
            }

            //Draw Selection are
            if (SelectionDisplay.width != 0 && SelectionDisplay.height != 0)
                UnityEngine.GUI.Label(SelectionDisplay, "", UnityEngine.GUI.skin.GetStyle("grey_border"));
        }

        public void DragSelection () {
            //TODO: Sometimes nodes might move with varying speeds
            //      Haven't found a way to reproduce this.
            if (SelectedNodes.Count < 2)
                return;

            NodeView draggedNode = null;
            foreach (var node in SelectedNodes) {
                if (node.IsDragged()) {
                    draggedNode = node;
                    break;
                }
            }
            if (draggedNode == null)
                return;
            foreach (var node in SelectedNodes) {
                if (node != draggedNode) {
                    node.DragNode(draggedNode.DragVector());
                    node.ClearDrag();
                }
            }

            draggedNode.ClearDrag();
        }

        public void UnselectAll () {
            foreach (NodeView node in SelectedNodes) {
                node.DeselectNode();
            }
            SelectedNodes.Clear();
        }

        public void SelectNodes (NodeView[] nodes, Rect selection) {
            if (selection.height < 0)
                selection = new Rect(selection.x, selection.y + selection.height, selection.width, System.Math.Abs(selection.height));

            if (selection.width < 0)
                selection = new Rect(selection.x + selection.width, selection.y, System.Math.Abs(selection.width), selection.height);

            foreach (var node in nodes) {
                var rect = node.GetRect();
                rect.position += new Vector2(0, 30);
                if (selection.Overlaps(rect)) {
                    SelectNode(node);
                }
            }
        }

        public NodeView SelectNode (NodeView[] nodes, Vector2 position) {
            foreach (var node in nodes) {
                var rect = node.GetRect();
                rect.position += new Vector2(0, 30);
                /*
                if (rect.Contains(position)) {
                    return node;
                }*/

                if (position.x > rect.x &&
                   position.x < rect.x + rect.width &&
                   position.y > rect.y &&
                   position.y < rect.y + rect.height) {
                    return node;
                }
            }
            return null;
        }

        public void SelectNode (NodeView node) {
            SelectedNodes.Add(node);
            node.SelectNode();
        }

        public bool IsNodeSelected (NodeView node) {
            foreach (var nodeData in SelectedNodes) {
                if (node.GetData().Guid == nodeData.GetData().Guid)
                    return true;
            }
            return false;
        }
    }
}