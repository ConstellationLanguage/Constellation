using System;
using Constellation;
using UnityEngine;
using UnityEditor;

namespace ConstellationEditor
{
    public class NodeView
    {
        public NodeData NodeData;
        private float previousNodePositionX;
        private float previousNodePositionY;
        private string nodeName;
        private float previousNodeSizeX;
        private float previousNodeSizeY;
        public const float nodeTitleHeight = 20;
        public const float nodeDeleteSize = 15;
        public const float resizeButtonSize = 10;
        public const float inputSize = 10;
        public const float outputSize = 10;
        public const float spacing = 10;
        public const float titleLeftMargin = 5;
        public const float titleRightMargin = 5;
        public const float leftAttributeMargin = 5;
        public const float rightAttributeMargin = 5;
        public const float attributeSpacing = 2;
        bool isAttributeValueChanged = false;
        bool wasMouseOverNode = false;
        Rect AtrributeSize = new Rect(0, 0, 88, 16);
        private string LastFocusedAttribute;

        public NodeView(NodeData node)
        {
            isAttributeValueChanged = false;
            NodeData = node;
            nodeName = node.Name;
            LockNodeSize();
            LockNodePosition();

            foreach (var attribute in node.AttributesData)
            {
                attribute.Value = AttributeStyleFactory.Reset(attribute.Type, attribute.Value);
            }
        }

        public void DrawNode(Event e, ConstellationEditorStyles constellationEditorStyle, System.Action<string> lockFocus, System.Action releaseFocus, string focusedNode)
        {
            var editorConfig = constellationEditorStyle;
            var nodeSizeX = GetSizeX();
            var nodeSizeY = GetSizeY();
            var nodePositionX = GetPositionX();
            var nodePositionY = GetPositionY();
            float positionOffsetX = nodeSizeX * 0.5f;
            float positionOffsetY = nodeSizeY * 0.5f;
            var nodeRect = new Rect(nodePositionX, nodePositionY, nodeSizeX, nodeSizeY);
            var nodeTitleRect = new Rect(nodePositionX + titleLeftMargin, nodePositionY, nodeSizeX - titleLeftMargin - titleRightMargin, nodeTitleHeight);
            var deleteRect = GetDeleteRect();
            var questionRect = GetQuestionRect();
            var resizeRect = GetResizeRect();
            var nodeSkin = editorConfig.NodeStyle;
            var nodeTitleSkin = editorConfig.NodeTitleStyle;
            var nodeResizeSkin = editorConfig.NodeResizeButtonStyle;
            GUI.Box(nodeRect, "", nodeSkin);
            GUI.Label(nodeTitleRect, GetName(), nodeTitleSkin);
            if (nodeRect.Contains(e.mousePosition) || (GUI.GetNameOfFocusedControl() == LastFocusedAttribute)) // check if mouse inside node or mouse focus attribute
            {
                if (focusedNode == NodeData.Guid || focusedNode == "")
                {
                    GUI.SetNextControlName(NodeData.Guid + "-" + "Delete rect");
                    GUI.color = new Color(0.75f, 0.75f, 0.75f);
                    GUI.Button(deleteRect, "X");
                    GUI.SetNextControlName(NodeData.Guid + "-" + "Question rect");
                    GUI.color = new Color(0.75f, 0.75f, 0.75f);
                    GUI.Button(questionRect, "?");
                    GUI.SetNextControlName(NodeData.Guid + "-" + "Resize rect");
                    GUI.color = Color.white;
                    GUI.Button(resizeRect, "", nodeResizeSkin);
                    lockFocus(NodeData.Guid);
                    wasMouseOverNode = true;
                }
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    releaseFocus();
                    GUI.FocusControl(null);
                    wasMouseOverNode = false;
                }
            } else if(wasMouseOverNode)
            {
                releaseFocus();
                GUI.FocusControl(null);
                wasMouseOverNode = false;
            }

            GUI.color = Color.white;

            var inputs = NodeData.GetInputs();
            var coldColor = new Color(1f, 0.9f, 0.1f);
            var warmColor = new Color(1f, 0.6f, 0.1f);
            var coldObjectColor = new Color(0.5f, 0.5f, 1f);
            for (var i = 0; i < inputs.Length; i++)
            {
                if (inputs[i].IsWarm == true)
                    GUI.color = editorConfig.GetConstellationIOStylesByType(inputs[i].Type).WarmColor;
                else
                    GUI.color = editorConfig.GetConstellationIOStylesByType(inputs[i].Type).ColdColor;
                    
                GUI.Button(GetInputRect(i), "", editorConfig.GetConstellationIOStylesByType(inputs[i].Type).InputStyle);
            }

            DrawAttributes(constellationEditorStyle);
            var outputs = NodeData.GetOutputs();
            for (var i = 0; i < outputs.Length; i++)
            {
                if (outputs[i].IsWarm == true)
                    GUI.color = editorConfig.GetConstellationIOStylesByType(outputs[i].Type).WarmColor;
                else
                    GUI.color = editorConfig.GetConstellationIOStylesByType(outputs[i].Type).ColdColor;

                GUI.Button(GetOuptputRect(i), "", editorConfig.GetConstellationIOStylesByType(outputs[i].Type).InputStyle);
            }
            GUI.color = Color.white;
        }

        public Rect GetInputRect(int InputID)
        {
            return new Rect(GetPositionX() - (inputSize * 0.25f), GetPositionY() + (InputID * (inputSize + spacing)) + nodeTitleHeight, inputSize, inputSize);
        }

        public Rect GetOuptputRect(int InputID)
        {
            return new Rect(GetPositionX() + GetSizeX() - (outputSize * 0.75f), GetPositionY() + (InputID * (outputSize + spacing)) + nodeTitleHeight, outputSize, outputSize);
        }

        public void UpdateNodeSize(float _x, float _y)
        {
            NodeData.SizeX = Math.Max(_x, MinimumNodeWidth());
            NodeData.SizeY = Math.Max(_y, MinimumNodeHeight());
        }

        public void SetPosition(float _x, float _y)
        {
            NodeData.XPosition = RoundToNearest(_x);
            NodeData.YPosition = RoundToNearest(_y);
        }

        public float MinimumNodeHeight()
        {
            var minimumHeight = 30;
            return Mathf.Max(Math.Max((inputSize + spacing) * NodeData.Inputs.Count + nodeTitleHeight, (inputSize + spacing) * NodeData.Outputs.Count + nodeTitleHeight), minimumHeight);
        }

        public float MinimumNodeWidth()
        {
            var minimumWidth = 100;
            if (NodeData.GetAttributes().Length == 0)
            {
                minimumWidth = 50;
            }
            return minimumWidth;
        }
        public void SetName(string _name)
        {
            nodeName = _name;
        }

        public string GetName()
        {
            return nodeName;
        }

        public float GetPositionX()
        {
            return NodeData.XPosition;
        }

        public float GetPositionY()
        {
            return NodeData.YPosition;
        }

        public float GetSizeX()
        {
            return NodeData.SizeX;
        }

        public float GetSizeY()
        {
            return NodeData.SizeY;
        }

        public void LockNodeSize()
        {
            previousNodeSizeX = NodeData.SizeX;
            previousNodeSizeY = NodeData.SizeY;
        }

        public InputData[] GetInputs()
        {
            return NodeData.GetInputs();
        }

        public OutputData[] GetOutputs()
        {
            return NodeData.GetOutputs();
        }

        public AttributeData[] GetAttributeDatas()
        {
            return NodeData.GetAttributes();
        }

        public void LockNodePosition()
        {
            previousNodePositionX = NodeData.XPosition;
            previousNodePositionY = NodeData.YPosition;
        }

        public float GetPreviousNodePositionX()
        {
            return previousNodePositionX;
        }

        public float GetPreviousNodePositionY()
        {
            return previousNodePositionY;
        }

        public float GetPreviousNodeSizeX()
        {
            return previousNodeSizeX;
        }

        public float GetPreviousNodeSizeY()
        {
            return previousNodeSizeY;
        }

        public Rect GetDeleteRect()
        {
            return new Rect(GetPositionX() + (GetSizeX() - nodeDeleteSize), GetPositionY(), nodeDeleteSize, nodeDeleteSize);
        }

        public Rect GetQuestionRect()
        {
            return new Rect(GetPositionX() + (GetSizeX() - nodeDeleteSize - nodeDeleteSize), GetPositionY(), nodeDeleteSize, nodeDeleteSize);
        }

        public Rect GetResizeRect()
        {
            return new Rect(GetPositionX() + GetSizeX() - resizeButtonSize, GetPositionY() + GetSizeY() - resizeButtonSize, resizeButtonSize, resizeButtonSize);
        }

        public Rect GetNodeRect(out float positionOffsetX, out float positionOffsetY)
        {
            var rect = new Rect(GetPositionX(), GetPositionY(), GetSizeX(), GetSizeY());
            positionOffsetX = GetSizeX() * 0.5f;
            positionOffsetY = GetSizeY() * 0.5f;
            return rect;
        }

        private void DrawAttributes(ConstellationEditorStyles editorStyles)
        {
            GUI.color = Color.white;
            if (NodeData.GetAttributes() != null)
            {
                var i = 0;
                foreach (var attribute in NodeData.AttributesData)
                {
                    var attributeControleName = NodeData.Guid + "-" + i;
                    var isFocusable = false;
                    EditorGUIUtility.labelWidth = 25;
                    EditorGUIUtility.fieldWidth = 10;
                    var attributeRect = GetAttributeRect(i);
                    var nodeAttributeRect = new Rect(NodeData.XPosition + 10, NodeData.YPosition + nodeTitleHeight, NodeData.SizeX - 20, NodeData.SizeY - nodeTitleHeight - 10);
                    if (attribute.Value != null)
                    {
                        GUI.SetNextControlName(attributeControleName);
                        var currentAttributeValue = attribute.Value.GetString();
                        attribute.Value = AttributeStyleFactory.Draw(attribute.Type, attributeRect, nodeAttributeRect, attribute.Value, editorStyles,out isFocusable);
                        if (attribute.Value != null)
                        {
                            if (currentAttributeValue != attribute.Value.GetString())
                                AttributeValueChanged();
                        }
                    }
                    if (GUI.GetNameOfFocusedControl() == attributeControleName && isFocusable)
                    {
                        LastFocusedAttribute = attributeControleName;
                    }
                    i++;
                }
            }
        }

        public Rect GetAttributeRect(int attributeID)
        {
            var leftOffset = inputSize + leftAttributeMargin;
            var rightOffset = rightAttributeMargin + outputSize + leftOffset;
            var topOffset = 3;
            return new Rect(NodeData.XPosition + leftOffset, NodeData.YPosition + ((AtrributeSize.height + attributeSpacing) * attributeID) + nodeTitleHeight - topOffset, NodeData.SizeX - rightOffset, AtrributeSize.height);
        }

        public float RoundToNearest(float a)
        {
            return a = a - (a % 5f);
        }

        public bool IsAttributeValueChanged()
        {
            var changeState = isAttributeValueChanged;
            isAttributeValueChanged = false;
            return changeState;
        }

        private void AttributeValueChanged()
        {
            isAttributeValueChanged = true;
        }
    }
}
