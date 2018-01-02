using Constellation;
using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {
    public class NodeView {
        private Rect Rect;
        public NodeData node;
        private NodeEditorPanel editor;
        private NodeConfig nodeConfig;
        private ConstellationScript constellationScript;
        private bool isDestroyed = false;
        private bool selected = false;
        private bool nodeMoved = false;
        private Vector2 nodeMovement = Vector2.zero;
        private bool DrawDescription = false;
        private string Description = "";
        private bool CloseOnNextFrame = false;
        private bool isAttributeValueChanged = false;

        public NodeView (NodeData _node, NodeEditorPanel _editor, NodeConfig _nodeConfig, ConstellationScript _constellation) {
            nodeConfig = _nodeConfig;
            var nodeWidth = nodeConfig.NodeWidth;
            if (_node.GetAttributes ().Length > 0) {
                nodeWidth = nodeConfig.NodeWidthAsAttributes;
            }
            Rect = new Rect (_node.XPosition, _node.YPosition, nodeWidth, (Mathf.Max (Mathf.Max (_node.Inputs.Count, _node.Outputs.Count), _node.AttributesData.Count) * nodeConfig.InputSize) + nodeConfig.TopMargin);
            node = _node;
            editor = _editor;
            constellationScript = _constellation;

            foreach (var attribute in node.AttributesData) {
                attribute.Value = AttributeStyleFactory.Reset (attribute.Type, attribute.Value);
            }
        }

        public void DrawWindow (int id, GUI.WindowFunction DrawNodeWindow, bool isNote) {
            if (DrawDescription)
                DrawHelp (Description);

            var defaultStyle = GUI.skin.GetStyle ("flow node 0");
            if (selected)
                defaultStyle = GUI.skin.GetStyle ("flow node 0 on");
            defaultStyle.alignment = TextAnchor.UpperRight;
            defaultStyle.margin.top = -5;

            if (node.Name != "Note")
                Rect = GUI.Window (id, Rect, DrawNodeWindow, "", defaultStyle);
            else
                Rect = GUI.Window (id, new Rect (Rect.x, Rect.y, 120, 120), DrawNodeWindow, "", GUI.skin.GetStyle ("VCS_StickyNote"));

            if (node.XPosition != Rect.x || node.YPosition != Rect.y) {
                nodeMovement = new Vector2 (node.XPosition - Rect.x, node.YPosition - Rect.y);
                nodeMoved = true;
            } else {
                nodeMovement = Vector2.zero;
                nodeMoved = false;
            }
            node.XPosition = Rect.x;
            node.YPosition = Rect.y;
        }

        public bool IsDragged () {
            return nodeMoved;
        }

        public Vector2 DragVector () {
            return nodeMovement;
        }

        public void DragNode (Vector2 vector) {
            Rect = new Rect (Rect.x - vector.x, Rect.y - vector.y, Rect.width, Rect.height);
            node.XPosition = Rect.x;
            node.YPosition = Rect.y;
        }

        public void ClearDrag () {
            nodeMoved = false;
            nodeMovement = Vector2.zero;

        }

        public void SelectNode () {
            selected = true;
        }

        public void DeselectNode () {
            selected = false;
        }

        public void DestroyNode () {
            constellationScript.RemoveNode (node);
            isDestroyed = true;
        }

        public bool NodeExist () {
            return !isDestroyed;
        }

        private void DrawHelp (string text) {
            Event current = Event.current;
            GUI.Label (new Rect (current.mousePosition.x, current.mousePosition.y, 120, 30), text, GUI.skin.GetStyle ("AnimationEventTooltip"));
            if (CloseOnNextFrame == true) {
                DrawDescription = false;
                CloseOnNextFrame = false;
            }
            if (current.isMouse) {
                CloseOnNextFrame = true;
            }
        }

        public bool IsAttributeValueChanged () {
            var changeState = isAttributeValueChanged;
            isAttributeValueChanged = false;
            return changeState;
        }

        private void AttributeValueChanged () {
            isAttributeValueChanged = true;
        }

        public void DrawContent () {
            if (node.GetAttributes () != null) {
                var i = 0;
                foreach (var attribute in node.AttributesData) {
                    EditorGUIUtility.labelWidth = 25;
                    EditorGUIUtility.fieldWidth = 10;
                    var attributeRect = new Rect (nodeConfig.AtrributeSize.x, nodeConfig.AtrributeSize.y + (nodeConfig.AtrributeSize.height * i), nodeConfig.AtrributeSize.width, nodeConfig.AtrributeSize.height);
                    if (attribute.Value != null) {
                        var currentAttributeValue = attribute.Value.GetString ();
                        attribute.Value = AttributeStyleFactory.Draw (attribute.Type, attributeRect, attribute.Value);
                        if (attribute.Value != null) {
                            if (currentAttributeValue != attribute.Value.GetString ())
                                AttributeValueChanged ();
                            i++;
                        }
                    }
                }
            }
            if (node.Inputs != null) {
                var i = 0;
                foreach (var input in node.Inputs) {
                    GUIStyle style;
                    if (input.IsWarm == true) {
                        if (input.Type == "Object")
                            style = nodeConfig.WarmInputObjectStyle;
                        else
                            style = nodeConfig.WarmInputStyle;
                    } else {
                        if (input.Type == "Object")
                            style = nodeConfig.ColdInputObjectStyle;
                        else
                            style = nodeConfig.ColdInputStyle;
                    }

                    if (GUI.Button (new Rect (0, nodeConfig.TopMargin + (nodeConfig.InputSize * i), nodeConfig.InputSize, nodeConfig.InputSize), "",
                            style)) {
                        Event current = Event.current;
                        if (current.button == 0)
                            editor.AddLinkFromInput (input);
                        else {
                            DrawDescription = true;
                            Description = input.Description;
                        }
                    }
                    i++;
                }
            }

            if (node.Outputs != null) {
                var i = 0;
                foreach (var output in node.Outputs) {
                    GUIStyle style;
                    if (output.IsWarm == true) {
                        if (output.Type == "Object")
                            style = nodeConfig.WarmInputObjectStyle;
                        else
                            style = nodeConfig.WarmInputStyle;
                    } else {
                        if (output.Type == "Object")
                            style = nodeConfig.ColdInputObjectStyle;
                        else
                            style = nodeConfig.ColdInputStyle;
                    }
                    if (GUI.Button (new Rect (Rect.width - nodeConfig.OutputSize, nodeConfig.TopMargin + ((nodeConfig.OutputSize) * i), nodeConfig.OutputSize, nodeConfig.OutputSize), "",
                            style)) {
                        Event current = Event.current;
                        if (current.button == 0)
                            editor.AddLinkFromOutput (output);
                        else {
                            DrawDescription = true;
                            Description = output.Description;
                        }
                    }
                    i++;
                }
            }

            GUI.Label (new Rect (40, 0, 100, 16), node.Name, UnityEngine.GUI.skin.GetStyle ("MiniLabel"));

            UnityEngine.GUI.Box (new Rect (0, 1, 20, 13), "", UnityEngine.GUI.skin.GetStyle ("sv_label_0"));
            if (GUI.Button (new Rect (4, 1, 13, 13), "", GUI.skin.GetStyle ("WinBtnClose"))) {
                DestroyNode ();
            }
            if (GUI.Button (new Rect (20, 1, 20, 13), "?", UnityEngine.GUI.skin.GetStyle ("sv_label_0"))) {
                NodeHelpWindow.ShowHelpWindow (node.Name);
            }
            if (DrawDescription)
                DrawHelp (Description);
        }
        public NodeData GetData () {
            return node;
        }

        public Rect GetRect () {
            return Rect;
        }
    }
}