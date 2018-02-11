using Constellation;
using UnityEditor;
using UnityEngine;

namespace ConstellationEditor {
    public class NodeView {
        private const int ButtonSize = 14;

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
        private bool isMouseOver = true;

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
            //Only draw visible nodes
            if (!editor.InView (Rect))
                return;

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

            //node name width. Modified when buttons are visible.
            var width = Rect.width - 10;

            UpdateMouseOverState(true);
            //Draw help and close button if mouse is over node
            if (MouseOver ()) {
                //Save original gui color
                var color = GUI.color;

                //Modify node name width to prevent overlapping with buttons
                width -= ButtonSize * 2 + 7;

                //Light gray color for close button
                GUI.color = new Color (0.8f, 0.8f, 0.8f);
                UnityEngine.GUI.Box (new Rect (Rect.width - (ButtonSize + 2), 1, ButtonSize, ButtonSize), "", UnityEngine.GUI.skin.GetStyle ("sv_label_0"));
                if (GUI.Button (new Rect (Rect.width - (ButtonSize + 1), 1, ButtonSize - 2, ButtonSize), "", GUI.skin.GetStyle ("WinBtnClose"))) {
                    DestroyNode ();
                }

                //The following could be simplified with custom GUIStyle?
                //Make invisible button
                GUI.color = new Color (0, 0, 0, 0);
                var helpPosition = new Rect (Rect.width - (ButtonSize * 2 + 5), 1, ButtonSize, ButtonSize);
                if (GUI.Button (helpPosition, "")) {
                    NodeHelpWindow.ShowHelpWindow (node.Name);
                }

                //Restore original gui color
                GUI.color = color;

                //Create help icon on top of invisible button
                Texture image = EditorGUIUtility.IconContent ("_Help").image;
                GUI.DrawTexture (helpPosition, image, ScaleMode.ScaleToFit);
            }

            //Draw node name
            GUI.Label (new Rect (10, 0, width, 16), node.Name, UnityEngine.GUI.skin.GetStyle ("MiniLabel"));

            if (DrawDescription)
                DrawHelp (Description);
        }

        private bool MouseOver () {
            //[TODO] the check on mouse over is having a conflict with gui.window had to disable it because it was buggy if the editor was at a low fps.
            //var current = Event.current.mousePosition;
            //return (current.x >= 0 && current.x <= Rect.width && current.y >= 0 && current.y <= Rect.height);
            return isMouseOver;
        }

        //I had to set the mouse over state after the update window was set because the event was preventing the gui.window from updating.
        public void UpdateMouseOverState (bool _mouseOverState) {
            isMouseOver = _mouseOverState;
        }

        public NodeData GetData () {
            return node;
        }

        public Rect GetRect () {
            return Rect;
        }
    }
}