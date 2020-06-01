using System;
using Constellation;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

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
        List<int> changedParameters;
        bool wasMouseOverNode = false;
        Rect AtrributeSize = new Rect(0, 0, 88, 16);
        private string LastFocusedAttribute;
        private bool isSelected = false;
        private Vector2 mousePosition;

        public NodeView(NodeData node)
        {
            changedParameters = new List<int>();
            NodeData = node;
            if (node.OverrideDisplayedName == "")
            {
                nodeName = node.Name;
            }else
            {
                nodeName = node.OverrideDisplayedName;
            }
            LockNodeSize();
            LockNodePosition();

            foreach (var attribute in node.ParametersData)
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
            var nodeTitleRect = new Rect(nodePositionX + constellationEditorStyle.titleLeftMargin, nodePositionY, nodeSizeX - constellationEditorStyle.titleLeftMargin - constellationEditorStyle.titleRightMargin, constellationEditorStyle.nodeTitleHeight);
            var deleteRect = GetDeleteRect(constellationEditorStyle);
            var questionRect = GetQuestionRect(constellationEditorStyle);
            var resizeRect = GetResizeRect(constellationEditorStyle);
            var nodeSkin = editorConfig.NodeStyle;
            if (isSelected)
                nodeSkin = editorConfig.NodeSelectedStyle;
            var nodeTitleSkin = editorConfig.NodeTitleStyle;
            var nodeResizeSkin = editorConfig.NodeResizeButtonStyle;
            GUI.Box(nodeRect, "", nodeSkin);
            GUI.Label(nodeTitleRect, GetName(), nodeTitleSkin);

            if (e.type == EventType.Repaint)
                mousePosition = e.mousePosition;

            if (nodeRect.Contains(mousePosition) || (GUI.GetNameOfFocusedControl() == LastFocusedAttribute)) // check if mouse inside node or mouse focus attribute
            {
                if (focusedNode == NodeData.Guid || focusedNode == "")
                {
                    GUI.SetNextControlName(NodeData.Guid + "-" + "Delete rect");
                    GUI.color = new Color(0.75f, 0.75f, 0.75f);
                    GUIStyle DeleteButton = constellationEditorStyle.GenericDeleteStyle;
                    GUI.Button(deleteRect, "", DeleteButton);
                    GUI.SetNextControlName(NodeData.Guid + "-" + "Question rect");
                    GUIStyle QuestionButton = constellationEditorStyle.GenericQuestionStyle;
                    GUI.color = new Color(0.75f, 0.75f, 0.75f);
                    GUI.Button(questionRect, "", QuestionButton);
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
            for (var i = 0; i < inputs.Length; i++)
            {
                if (inputs[i].IsBright == true)
                    GUI.color = editorConfig.GetConstellationIOStylesByType(inputs[i].Type).WarmColor;
                else
                    GUI.color = editorConfig.GetConstellationIOStylesByType(inputs[i].Type).ColdColor;
                    
                GUI.Button(GetInputRect(i, constellationEditorStyle), "", editorConfig.GetConstellationIOStylesByType(inputs[i].Type).InputStyle);
            }

            DrawParameters(constellationEditorStyle);
            var outputs = NodeData.GetOutputs();
            for (var i = 0; i < outputs.Length; i++)
            {
                if (outputs[i].IsBright == true)
                    GUI.color = editorConfig.GetConstellationIOStylesByType(outputs[i].Type).WarmColor;
                else
                    GUI.color = editorConfig.GetConstellationIOStylesByType(outputs[i].Type).ColdColor;

                GUI.Button(GetOuptputRect(i, constellationEditorStyle), "", editorConfig.GetConstellationIOStylesByType(outputs[i].Type).InputStyle);
            }
            GUI.color = Color.white;
        }

        public Rect GetInputRect(int InputID, ConstellationEditorStyles constellationEditorStyles)
        {
            return new Rect(GetPositionX() - (constellationEditorStyles.inputSize * 0.25f), GetPositionY() + (InputID * (constellationEditorStyles.inputSize + constellationEditorStyles.spacing)) + constellationEditorStyles.nodeTitleHeight, constellationEditorStyles.inputSize, constellationEditorStyles.inputSize);
        }

        public Rect GetOuptputRect(int InputID, ConstellationEditorStyles constellationEditorStyles)
        {
            return new Rect(GetPositionX() + GetSizeX() - (constellationEditorStyles.outputSize * 0.75f), GetPositionY() + (InputID * (constellationEditorStyles.outputSize + constellationEditorStyles.spacing)) + constellationEditorStyles.nodeTitleHeight, constellationEditorStyles.outputSize, constellationEditorStyles.outputSize);
        }

        public void UpdateNodeSize(float _x, float _y, ConstellationEditorStyles constellationEditorStyles)
        {
            NodeData.SizeX = Math.Max(_x, MinimumNodeWidth());
            NodeData.SizeY = Math.Max(_y, MinimumNodeHeight(constellationEditorStyles));
        }

        public void SetPosition(float _x, float _y)
        {
            NodeData.XPosition = RoundToNearest(_x);
            NodeData.YPosition = RoundToNearest(_y);
        }

        public float MinimumNodeHeight(ConstellationEditorStyles constellationEditorStyles)
        {
            var minimumHeight = 30;
            return Mathf.Max(Math.Max((constellationEditorStyles.inputSize + constellationEditorStyles.spacing) * NodeData.Inputs.Count + constellationEditorStyles.nodeTitleHeight, (constellationEditorStyles.inputSize + constellationEditorStyles.spacing) * NodeData.Outputs.Count + constellationEditorStyles.nodeTitleHeight), minimumHeight);
        }

        public float MinimumNodeWidth()
        {
            var minimumWidth = 100;
            if (NodeData.GetParameters().Length == 0)
            {
                minimumWidth = 75;
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

        public ParameterData[] GetAttributeDatas()
        {
            return NodeData.GetParameters();
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

        public Rect GetDeleteRect(ConstellationEditorStyles editorStyles)
        {
            return new Rect(GetPositionX() + (GetSizeX() - editorStyles.nodeDeleteSize), GetPositionY() + editorStyles.nodeButtonsTopMargin, editorStyles.nodeDeleteSize, editorStyles.nodeDeleteSize);
        }

        public Rect GetQuestionRect(ConstellationEditorStyles editorStyles)
        {
            return new Rect(GetPositionX() + (GetSizeX() - editorStyles.nodeDeleteSize - editorStyles.nodeDeleteSize) - editorStyles.nodeButtonsSpacing, GetPositionY() + editorStyles.nodeButtonsTopMargin, editorStyles.nodeDeleteSize, editorStyles.nodeDeleteSize);
        }

        public Rect GetResizeRect(ConstellationEditorStyles editorStyles)
        {
            return new Rect(GetPositionX() + GetSizeX() - editorStyles.resizeButtonSize, GetPositionY() + GetSizeY() - editorStyles.resizeButtonSize, editorStyles.resizeButtonSize, editorStyles.resizeButtonSize);
        }

        public Rect GetNodeRect(out float positionOffsetX, out float positionOffsetY)
        {
            var rect = new Rect(GetPositionX(), GetPositionY(), GetSizeX(), GetSizeY());
            positionOffsetX = GetSizeX() * 0.5f;
            positionOffsetY = GetSizeY() * 0.5f;
            return rect;
        }

        private void DrawParameters(ConstellationEditorStyles editorStyles)
        {
            GUI.color = Color.white;
            if (NodeData.GetParameters() != null)
            {
                var i = 0;
                foreach (var parameter in NodeData.ParametersData)
                {
                    var parameterControleName = NodeData.Guid + "-" + i;
                    var isFocusable = false;
                    EditorGUIUtility.labelWidth = 25;
                    EditorGUIUtility.fieldWidth = 10;
                    var parameterRect = GetParameterRect(i, editorStyles);
                    var nodeAttributeRect = new Rect(NodeData.XPosition + 10, NodeData.YPosition + editorStyles.nodeTitleHeight, NodeData.SizeX - 20, NodeData.SizeY - editorStyles.nodeTitleHeight - 10);
                    if (parameter.Value != null)
                    {
                        GUI.SetNextControlName(parameterControleName);
                        var currentParameterValue = parameter.Value.GetString();
                        parameter.Value = AttributeStyleFactory.Draw(parameter.Type, parameterRect, nodeAttributeRect, parameter.Value, editorStyles,out isFocusable);
                        if (parameter.Value != null)
                        {
                            if (currentParameterValue != parameter.Value.GetString())
                                ParameterValueChanged(i);
                        }
                    }
                    if (GUI.GetNameOfFocusedControl() == parameterControleName && isFocusable)
                    {
                        LastFocusedAttribute = parameterControleName;
                    }
                    i++;
                }
            }
        }

        public Rect GetParameterRect(int attributeID, ConstellationEditorStyles constellationEditorStyles)
        {
            var leftOffset = constellationEditorStyles.inputSize + constellationEditorStyles.leftAttributeMargin;
            var rightOffset = constellationEditorStyles.rightAttributeMargin + constellationEditorStyles.outputSize + leftOffset;
            var topOffset = 3;
            return new Rect(NodeData.XPosition + leftOffset, NodeData.YPosition + ((AtrributeSize.height + constellationEditorStyles.attributeSpacing) * attributeID) + constellationEditorStyles.nodeTitleHeight - topOffset, NodeData.SizeX - rightOffset, AtrributeSize.height);
        }

        public float RoundToNearest(float a)
        {
            return a = a - (a % 5f);
        }

        public int [] IsParameterValueChanged()
        {
            var changeState = changedParameters.ToArray();
            changedParameters.Clear();
            return changeState;
        }

        public void SelectedNode()
        {
            isSelected = true;
        }

        public void UnselectNode()
        {
            isSelected = false;
        }

        private void ParameterValueChanged(int id)
        {
            changedParameters.Add(id);
        }
    }
}
