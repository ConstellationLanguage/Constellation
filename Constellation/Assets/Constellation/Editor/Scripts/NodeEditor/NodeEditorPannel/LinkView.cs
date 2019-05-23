using Constellation;
using UnityEditor;
using UnityEngine;

namespace ConstellationEditor
{
    public class LinkView
    {
        private ConstellationScript constellationScript;
        private NodeEditorPanel editor;
        private NodeConfig nodeConfig;
        private bool dragging;
        public delegate void LinkRemoved(LinkData link);
        LinkRemoved OnLinkRemoved;

        public LinkView(IGUI _gui, NodeEditorPanel _editor, ConstellationScript _constellationScript, NodeConfig _nodeConfig, LinkRemoved _onLinkRemoved)
        {
            constellationScript = _constellationScript;
            editor = _editor;
            nodeConfig = _nodeConfig;
            OnLinkRemoved += _onLinkRemoved;
        }

        public LinkData[] GetLinks()
        {
            return constellationScript.GetLinks();
        }

        public void DrawLinks()
        {

            foreach (LinkData link in constellationScript.GetLinks())
            {
                Rect startLink = Rect.zero;
                Rect endLink = Rect.zero;
                foreach (NodeData node in constellationScript.GetNodes())
                {
                    var i = 1;
                    foreach (InputData input in node.GetInputs())
                    {
                        if (link.Input.Guid == input.Guid)
                        {
                            endLink = new Rect(node.XPosition,
                                node.YPosition + (nodeConfig.TopMargin * 0.5f) + ((nodeConfig.InputSize) * i),
                                0,
                                0);
                            break;
                        }
                        i++;
                    }

                    var j = 1;
                    foreach (OutputData output in node.GetOutputs())
                    {
                        if (link.Output.Guid == output.Guid)
                        {
                            var width = nodeConfig.NodeWidth;
                            if (node.GetAttributes().Length > 0)
                            {
                                width = nodeConfig.NodeWidthAsAttributes;
                            }

                            startLink = new Rect(node.XPosition + width,
                                node.YPosition + (nodeConfig.TopMargin * 0.5f) + ((nodeConfig.InputSize) * j),
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
                    OnLinkRemoved(link);
                }

                DrawNodeCurve(startLink, endLink, nodeConfig.GetConnectionColor(link.Input.IsWarm, link.Input.Type));

                if (MouseOverCurve(startLink.position, endLink.position))
                {
                    var linkCenter = new Rect((startLink.x + (endLink.x - startLink.x) / 2) - (nodeConfig.TopMargin * 0.5f),
                        (startLink.y + (endLink.y - startLink.y) / 2) - (nodeConfig.TopMargin * 0.5f),
                        nodeConfig.LinkButtonSize,
                        nodeConfig.LinkButtonSize);
                    GUI.Box(linkCenter, "", nodeConfig.HexagonButton);
                    GUI.Button(linkCenter, "", nodeConfig.CloseButton);

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
                                    OnLinkRemoved(link);
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
                            node.YPosition + (nodeConfig.TopMargin * 0.5f) + ((nodeConfig.InputSize) * i),
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
                        return new Rect(node.XPosition + nodeConfig.NodeWidth,
                            node.YPosition + (nodeConfig.TopMargin * 0.5f) + ((nodeConfig.InputSize) * j),
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

            if (!editor.InView(PointsToRect(startPos, endPos)))
                return;

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
    }
}