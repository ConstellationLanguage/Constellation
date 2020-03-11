﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Constellation;
using UnityEditor;

[System.Serializable]
public class LinksView
{
    public ConstellationScript constellationScript;
    public bool dragging;
    public bool isInstance;

    public InputData selectedInput;
    public OutputData selectedOutput;

    public Rect AtrributeSize = new Rect(18, 15, 88, 20);
    public Color WarmInputColor = new Color(0.8f, 0.5f, 0.3f);
    public Color ColdInputColor = Color.yellow;
    public Color WarmInputObjectColor = new Color(0.2f, 0.6f, 0.55f);
    public Color ColdInputObjectColor = new Color(0.2f, 0.3f, 0.6f);

    //const int topMargin = 30;
    //const int inputSize = 11;
    //const int nodeWidth = 100;
    const int deleteButtonSize = 15;

    public LinksView(ConstellationScript _constellationScript)
    {
        constellationScript = _constellationScript;
    }

    public LinkData[] GetLinks()
    {
        return constellationScript.GetLinks();
    }

    public void DrawLinks(ConstellationEditorCallbacks.RequestRepaint requestRepaint)
    {
        DrawIncompleteLink(requestRepaint);

        foreach (LinkData link in constellationScript.GetLinks())
        {
            Rect startLink = Rect.zero;
            Rect endLink = Rect.zero;
            foreach (NodeData node in constellationScript.GetNodes())
            {
                var i = 0;
                foreach (InputData input in node.GetInputs())
                {
                    var centerInputPosition = node.YPosition + (NodeView.nodeTitleHeight) + ((NodeView.inputSize + NodeView.spacing) * i) + (NodeView.inputSize *0.5f);
                    if (link.Input.Guid == input.Guid)
                    {
                        endLink = new Rect(node.XPosition,
                           centerInputPosition,
                            0,
                            0);
                        break;
                    }
                    i++;
                }

                var j = 0;
                foreach (OutputData output in node.GetOutputs())
                {
                    var centerInputPosition = node.YPosition + (NodeView.nodeTitleHeight) + ((NodeView.outputSize + NodeView.spacing) * i) + (NodeView.outputSize * 0.5f);
                    if (link.Output.Guid == output.Guid)
                    {
                        var width = node.SizeX;
                        startLink = new Rect(node.XPosition + width,
                            centerInputPosition,
                            0,
                            0);
                        break;
                    }
                    j++;
                }
            }
            if (startLink == Rect.zero || endLink == Rect.zero)
            {
                constellationScript.RemoveLink(link);
                //OnLinkRemoved(link);
            }

            DrawNodeCurve(startLink, endLink, GetConnectionColor(link.Input.IsWarm, link.Input.Type));

            if (MouseOverCurve(startLink.position, endLink.position))
            {
                
                var linkCenter = new Rect((startLink.x + (endLink.x - startLink.x) / 2) - (deleteButtonSize * 0.5f),
                    (startLink.y + (endLink.y - startLink.y) / 2) - (deleteButtonSize * 0.5f),
                    deleteButtonSize,
                    deleteButtonSize);
                GUI.Box(linkCenter, "X");
                GUI.Button(linkCenter, "X");
                if (Event.current.IsUsed())
                {
                    if (Event.current.button == 0)
                    {
                        if (!dragging)
                        {
                            dragging = true;
                            if (linkCenter.Contains(Event.current.mousePosition))
                            {
                                constellationScript.RemoveLink(link);
                            }
                        }
                    }
                }
                else if (!Event.current.IsLayoutOrRepaint())
                {
                    dragging = false;
                }
            }
        }
        requestRepaint();
    }

    public Rect InputPosition(InputData _input)
    {
        foreach (NodeData node in constellationScript.GetNodes())
        {
            var i = 1;
            foreach (InputData input in node.GetInputs())
            {
                if (_input.Guid == input.Guid)
                {
                    return new Rect(node.XPosition,
                        node.YPosition + (NodeView.nodeTitleHeight * 0.5f) + ((NodeView.inputSize + NodeView.spacing) * i),
                        0,
                        0);
                }
                i++;
            }
        }
        return Rect.zero;
    }

    public Rect OutputPosition(OutputData _output)
    {
        foreach (NodeData node in constellationScript.GetNodes())
        {
            var j = 1;
            foreach (OutputData output in node.GetOutputs())
            {
                if (_output.Guid == output.Guid)
                {
                    return new Rect(node.XPosition + node.SizeX,
                        node.YPosition + (NodeView.nodeTitleHeight * 0.5f) + ((NodeView.inputSize + NodeView.spacing) * j),
                        0,
                        0);
                }
                j++;
            }
        }
        return Rect.zero;
    }

    public void DrawNodeCurve(Rect start, Rect end)
    {
        DrawNodeCurve(start, end, Color.gray);
    }

    public void DrawNodeCurve(Rect start, Rect end, Color color)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);

        /*if (!editor.InView(PointsToRect(startPos, endPos)))
            return;*/

        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;

        //Smoother bezier curve for close distance
        var distance = Vector3.Distance(startPos, endPos);
        if (distance < 100)
        {
            startTan = startPos + Vector3.right * (distance * 0.5f);
            endTan = endPos + Vector3.left * (distance * 0.5f);
        }

        Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 5);
    }

    public bool MouseOverCurve(Vector3 start, Vector3 end)
    {
        //Currently creates rect to detect mouse over so it's nowhere near pixel perfect detection

        var mouse = Event.current.mousePosition;

        //Padding is needed to recognise straight lines
        var padding = 10;

        var startXFirst = (start.x < end.x);
        var startYFirst = (start.y < end.y);

        var mouseOverX = startXFirst ?
            mouse.x > start.x && mouse.x < end.x : mouse.x > end.x && mouse.x < start.x;

        var mouseOverY = startYFirst ?
            mouse.y + padding > start.y && mouse.y - padding < end.y : mouse.y + padding > end.y && mouse.y - padding < start.y;

        return (mouseOverX && mouseOverY);
    }

    private Rect PointsToRect(Vector3 start, Vector3 end)
    {
        return new Rect
        {
            x = (start.x < end.x) ? start.x : end.x,
            y = (start.y < end.y) ? end.y : start.y,
            width = Mathf.Abs(start.x - end.x),
            height = Mathf.Abs(start.y - end.y)
        };
    }

    public Color GetConnectionColor(bool _isWarm, string _type)
    {
        if (_isWarm)
        {
            if (_type == "Object")
                return WarmInputObjectColor;
            else
                return WarmInputColor;
        }
        else
        {
            if (_type == "Object")
                return ColdInputObjectColor;
            else
                return ColdInputColor;
        }
    }

    public void AddLinkFromOutput(OutputData _output)
    {
        if (selectedInput != null)
            CreateLink(selectedInput, _output);
        else if (selectedOutput == null)
            selectedOutput = _output;

    }

    public void AddLinkFromInput(InputData _input)
    {
        if (selectedOutput != null)
            CreateLink(_input, selectedOutput);
        else if (selectedInput == null)
            selectedInput = _input;
    }

    public void CreateLink(InputData _input, OutputData _output)
    {
        if (isInstance)
            constellationScript.IsDifferentThanSource = true;

        selectedInput = null;
        selectedOutput = null;
        var newLink = new LinkData(_input, _output);
        if (constellationScript.IsLinkValid(newLink))
        {
            constellationScript.AddLink(newLink);
            //OnLinkAdded(newLink);
            //undoable.AddAction();
            //GUI.RequestRepaint();
        }
    }

    private void DrawIncompleteLink(ConstellationEditorCallbacks.RequestRepaint requestRepaint)
    {
        if (selectedInput != null || selectedOutput != null)
        {
            var e = Event.current;
            if (selectedInput != null)
            {
                DrawNodeCurve(new Rect(e.mousePosition.x, e.mousePosition.y, 0, 0), InputPosition(selectedInput));
                requestRepaint();
            }
            else if (selectedOutput != null)
            {
                DrawNodeCurve(OutputPosition(selectedOutput), new Rect(e.mousePosition.x, e.mousePosition.y, 0, 0));
                requestRepaint();
            }

            if (e.button == 1)
            {
                selectedInput = null;
                selectedOutput = null;
            }
        }
    }
}
