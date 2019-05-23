using System.Collections.Generic;
using Constellation;
using UnityEngine;

namespace ConstellationEditor {
    public class NodeEditorSelection {
        public List<NodeView> SelectedNodes;
        private Vector2 StartMousePosition;
        private Vector2 DragSize;
        private Vector2 SelectionOffset;
        private IGUI GUI;
        private Vector2 offset;

        public NodeEditorSelection (IGUI gui, ClipBoard clipBoard) {
            SelectedNodes = new List<NodeView> ();
            StartMousePosition = Vector2.zero;
            DragSize = Vector2.zero;
            GUI = gui;
        }

        public void DestroySelection () {

            foreach (var node in SelectedNodes) {
                node.DestroyNode ();
            }
            UnselectAll ();
        }

        public void Draw (NodeView[] nodes, LinkData[] links, Vector2 selectionOffset, Rect window) {
            var current = Event.current;
            var hoverPosition = current.mousePosition + selectionOffset;
            var viewport = GUILayoutUtility.GetLastRect ();

            DragSelection ();

            if (!current.IsLayout ()) {
                offset = viewport.position;
            }

            UpdateKeyDown ();

            //Special handling for single node. This is to make GUI.DragWindow work correctly
            if (SelectedNodes.Count == 1 && DragSize == Vector2.zero) {
                if (!current.IsUsed () && !current.IsLayoutOrRepaint ()) {
                    UnselectAll ();
                    SelectNode (nodes, hoverPosition);
                    GUI.RequestRepaint ();
                }
                return;
            }

            //Deselect all
            if (current.MouseButtonDown (1) && current.isMouse) {
                UnselectAll ();
                UnityEngine.GUI.FocusControl(null);
                GUI.RequestRepaint ();
            }

            if (current.mousePosition.x + window.x < window.width 
                && current.mousePosition.y - window.y < window.height)
                UpdateSelectionArea (selectionOffset, Event.current, nodes);
            else {
                DragSize = Vector2.zero;
                StartMousePosition = Vector2.zero;
                DragSize = Vector2.zero;
            }

            //Mouse over
            if (DragSize == Vector2.zero && !current.IsUsed () && !current.MouseDrag ()) {
                if (SelectedNodes.Count <= 1) {
                    UnselectAll ();
                    var selection = SelectNode (nodes, hoverPosition);
                    if (selection != null)
                        SelectNode (selection);
                }
            }
        }

        private void UpdateKeyDown () {
            var current = Event.current;
            if (current.type != EventType.KeyDown) return;

            if (current.keyCode == KeyCode.Delete)
                DestroySelection ();
        }

        private void UpdateSelectionArea (Vector2 _offset, Event _event, NodeView[] _nodes) {
            var SelectionDisplay = new Rect (StartMousePosition.x, StartMousePosition.y, DragSize.x, DragSize.y);
            var Selection = new Rect (StartMousePosition.x + _offset.x, StartMousePosition.y + _offset.y, DragSize.x, DragSize.y);

            if (_event.MouseButtonDown (0) && _event.isMouse) {
                UnselectAll ();
                StartMousePosition = _event.mousePosition;
                GUI.RequestRepaint ();
            } else if (_event.button == 0 && _event.MouseDrag () && Selection.x != 0 && Selection.y != 0) {
                DragSize = _event.mousePosition - StartMousePosition;
                UnselectAll ();
                GUI.RequestRepaint ();
            } else if (Selection.x != 0 && Selection.y != 0 && Selection.width != 0 && Selection.height != 0) {
                if (_event.type == EventType.MouseUp) {
                    SelectNodes (_nodes, Selection);
                    StartMousePosition = Vector2.zero;
                    DragSize = Vector2.zero;
                    SelectionDisplay = Rect.zero;
                    GUI.RequestRepaint ();
                }
            }

            //Draw Selection area
            if (SelectionDisplay.width != 0 && SelectionDisplay.height != 0)
                UnityEngine.GUI.Label (SelectionDisplay, "", UnityEngine.GUI.skin.GetStyle ("grey_border"));
        }

        public void DragSelection () {

            if (SelectedNodes.Count < 2)
                return;

            NodeView draggedNode = null;
            foreach (var node in SelectedNodes) {
                if (node.IsDragged ()) {
                    draggedNode = node;
                    break;
                }
            }
            if (draggedNode == null)
                return;
            foreach (var node in SelectedNodes) {
                if (node != draggedNode) {
                    node.DragNode (draggedNode.DragVector ());
                    node.ClearDrag ();
                }
            }

            draggedNode.ClearDrag ();
        }

        public void UnselectAll () {
            foreach (NodeView node in SelectedNodes) {
                node.DeselectNode ();
            }
            SelectedNodes.Clear ();
        }

        public void SelectNodes (NodeView[] nodes, Rect selection) {
            if (selection.height < 0)
                selection = new Rect (selection.x, selection.y + selection.height, selection.width, System.Math.Abs (selection.height));

            if (selection.width < 0)
                selection = new Rect (selection.x + selection.width, selection.y, System.Math.Abs (selection.width), selection.height);

            foreach (var node in nodes) {
                var rect = node.GetRect ();
                rect.position += offset;
                if (selection.Overlaps (rect)) {
                    SelectNode (node);
                }
            }
        }

        public NodeView SelectNode (NodeView[] nodes, Vector2 position) {
            var possibleNodes = new List<NodeView> ();
            foreach (var node in nodes) {
                var rect = node.GetRect ();
                rect.position += offset;

                if (rect.Contains (position))
                    possibleNodes.Add (node);
            }

            if (possibleNodes.Count == 0)
                return null;
            else
                return possibleNodes[0];
            //TODO: sort out which node is on top currently
        }

        public void SelectNode (NodeView node) {
            SelectedNodes.Add (node);
            node.SelectNode ();
        }

        public bool IsNodeSelected (NodeView node) {
            foreach (var nodeData in SelectedNodes) {
                if (node.GetData ().Guid == nodeData.GetData ().Guid)
                    return true;
            }
            return false;
        }
    }
}