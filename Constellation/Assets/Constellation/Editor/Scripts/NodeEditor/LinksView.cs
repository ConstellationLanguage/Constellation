using UnityEngine;
using Constellation;
using UnityEditor;
using Constellation.Unity3D;

namespace ConstellationEditor
{
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
        const int deleteButtonSize = 15;
        private ConstellationEditorRules constellationRules;

        public LinksView(ConstellationScript _constellationScript, ConstellationEditorRules _constellationRules)
        {
            constellationScript = _constellationScript;
            constellationRules = _constellationRules;
        }

        public LinkData[] GetLinks()
        {
            return constellationScript.GetLinks();
        }

        public void DrawLinks(ConstellationEditorEvents.RequestRepaint requestRepaint, ConstellationEditorEvents.EditorEvents editorEvents, ConstellationEditorStyles styles)
        {
            DrawIncompleteLink(requestRepaint, styles);

            foreach (LinkData link in constellationScript.GetLinks())
            {
                Rect startLink = Rect.zero;
                Rect endLink = Rect.zero;
                foreach (NodeData node in constellationScript.GetNodes())
                {
                    var i = 0;
                    foreach (InputData input in node.GetInputs())
                    {
                        if (link.Input.Guid == input.Guid)
                        {
                            endLink = InputPosition(input, styles);
                            break;
                        }
                        i++;
                    }

                    var j = 0;
                    foreach (OutputData output in node.GetOutputs())
                    {
                        if (link.Output.Guid == output.Guid)
                        {
                            var width = node.SizeX;
                            startLink = OutputPosition(output, styles);
                            break;
                        }
                        j++;
                    }
                }
                if (startLink == Rect.zero || endLink == Rect.zero)
                {
                    editorEvents(ConstellationEditorEvents.EditorEventType.AddToUndo, "Delete Link");
                    editorEvents(ConstellationEditorEvents.EditorEventType.LinkDeleted, link.GUID);
                    constellationScript.RemoveLink(link);
                }

                DrawNodeCurve(startLink, endLink, GetConnectionColor(link.Input.IsBright, link.Output.Type, styles));

                if (MouseOverCurve(startLink.position, endLink.position))
                {
                    var linkCenter = new Rect((startLink.x + (endLink.x - startLink.x) / 2) - (deleteButtonSize * 0.5f),
                        (startLink.y + (endLink.y - startLink.y) / 2) - (deleteButtonSize * 0.5f),
                        deleteButtonSize,
                        deleteButtonSize);
                    var deleteButtonStyle = styles.GenericDeleteStyle;
                    if (GUI.Button(linkCenter, "", deleteButtonStyle))
                    {
                        dragging = true;
                        if (linkCenter.Contains(Event.current.mousePosition))
                        {
                            editorEvents(ConstellationEditorEvents.EditorEventType.LinkDeleted, link.GUID);
                            constellationScript.RemoveLink(link);
                        }
                    }
                }
            }
            requestRepaint();
        }

        public Rect InputPosition(InputData _input, ConstellationEditorStyles constellationEditorStyles)
        {
            foreach (NodeData node in constellationScript.GetNodes())
            {
                var i = 0;
                foreach (InputData input in node.GetInputs())
                {
                    if (_input.Guid == input.Guid)
                    {
                        return new Rect(node.XPosition,
                            node.YPosition + constellationEditorStyles.nodeTitleHeight + ((constellationEditorStyles.inputSize + constellationEditorStyles.spacing) * i) + (constellationEditorStyles.inputSize * 0.5f),
                            0,
                            0);
                    }
                    i++;
                }
            }
            return Rect.zero;
        }

        public Rect OutputPosition(OutputData _output, ConstellationEditorStyles constellationEditorStyles)
        {
            foreach (NodeData node in constellationScript.GetNodes())
            {
                var j = 0;
                foreach (OutputData output in node.GetOutputs())
                {
                    if (_output.Guid == output.Guid)
                    {
                        return new Rect(node.XPosition + node.SizeX,
                            node.YPosition + constellationEditorStyles.nodeTitleHeight + ((constellationEditorStyles.outputSize + constellationEditorStyles.spacing) * j) + (constellationEditorStyles.outputSize * 0.5f),
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

        public Color GetConnectionColor(bool _isWarm, string _type, ConstellationEditorStyles styles)
        {
            if (_isWarm)
                return styles.GetConstellationIOStylesByType(_type).WarmColor;
            else
                return styles.GetConstellationIOStylesByType(_type).ColdColor;
        }

        public void AddLinkFromOutput(OutputData _output, ConstellationEditorEvents.EditorEvents editorEvents)
        {
            if (constellationRules.AddLink(selectedInput, _output, constellationScript.script, () =>
            {
                editorEvents(ConstellationEditorEvents.EditorEventType.AddToUndo, "Added link");
            }, (string linkGUID) =>
            {
                editorEvents(ConstellationEditorEvents.EditorEventType.LinkAdded, linkGUID);
            }))
            {
                if (isInstance)
                    constellationScript.IsDifferentThanSource = true;
                
                selectedInput = null;
                selectedOutput = null;
            } else if (selectedOutput == null)
                selectedOutput = _output;

        }

        public void AddLinkFromInput(InputData _input, ConstellationEditorEvents.EditorEvents editorEvents)
        {
            if (constellationRules.AddLink(_input, selectedOutput, constellationScript.script, () =>
            {
                editorEvents(ConstellationEditorEvents.EditorEventType.AddToUndo, "Added link");
            }, (string linkGUID) =>
            {
                editorEvents(ConstellationEditorEvents.EditorEventType.LinkAdded, linkGUID);
            }))
            {
                if (isInstance)
                    constellationScript.IsDifferentThanSource = true;

                selectedInput = null;
                selectedOutput = null;
            }
            else if (selectedInput == null)
                selectedInput = _input;
        }

        private void DrawIncompleteLink(ConstellationEditorEvents.RequestRepaint requestRepaint, ConstellationEditorStyles styles)
        {
            if (selectedInput != null || selectedOutput != null)
            {
                var e = Event.current;
                if (selectedInput != null)
                {
                    DrawNodeCurve(new Rect(e.mousePosition.x, e.mousePosition.y, 0, 0), InputPosition(selectedInput, styles));
                    requestRepaint();
                }
                else if (selectedOutput != null)
                {
                    DrawNodeCurve(OutputPosition(selectedOutput, styles), new Rect(e.mousePosition.x, e.mousePosition.y, 0, 0));
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
}

