using Constellation;
using UnityEngine;
using UnityEditor;

namespace ConstellationEditor {
    public class LinkView {
        private ConstellationScript constellationScript;
        private NodeConfig nodeConfig;

        public LinkView (IGUI _gui, ConstellationScript _constellationScript, NodeConfig _nodeConfig) {
            constellationScript = _constellationScript;
            nodeConfig = _nodeConfig;
        }

        public LinkData [] GetLinks () 
        {
            return constellationScript.GetLinks();
        }

        public void DrawLinks () {
            foreach (LinkData link in constellationScript.GetLinks ()) {
                Rect startLink = Rect.zero;
                Rect endLink = Rect.zero;
                foreach (NodeData node in constellationScript.GetNodes ()) {
                    var i = 1;
                    foreach (InputData input in node.GetInputs ()) {
                        if (link.Input.Guid == input.Guid) {
                            endLink = new Rect (node.XPosition,
                                node.YPosition + (nodeConfig.TopMargin * 0.5f) + ((nodeConfig.InputSize) * i),
                                0,
                                0);
                        }
                        i++;
                    }

                    var j = 1;
                    foreach (OutputData output in node.GetOutputs ()) {
                        if (link.Output.Guid == output.Guid) {
                            var width = nodeConfig.NodeWidth;
                            if (node.GetAttributes ().Length > 0) {
                                width = nodeConfig.NodeWidthAsAttributes;
                            }

                            startLink = new Rect (node.XPosition + width,
                                node.YPosition + (nodeConfig.TopMargin*0.5f) + ((nodeConfig.InputSize) * j),
                                0,
                                0);
                        }
                        j++;
                    }
                }
                if (startLink == Rect.zero || endLink == Rect.zero) // the source has been destroyed
                    constellationScript.RemoveLink (link);

                link.outputPositionY = endLink.y;
                DrawNodeCurve (startLink, endLink);
                var linkCenter = new Rect ((startLink.x + (endLink.x - startLink.x) / 2) - (nodeConfig.TopMargin * 0.5f),
                    (startLink.y + (endLink.y - startLink.y) / 2) - (nodeConfig.TopMargin * 0.5f),
                    nodeConfig.LinkButtonSize,
                    nodeConfig.LinkButtonSize);

                UnityEngine.GUI.Box (linkCenter, "", UnityEngine.GUI.skin.GetStyle ("flow var 0"));
                if (UnityEngine.GUI.Button (linkCenter, "", UnityEngine.GUI.skin.GetStyle ("WinBtnClose"))) {
                    constellationScript.RemoveLink (link);
                }
            }
        }

        public Rect InputPosition (InputData _input) {
            foreach (NodeData node in constellationScript.GetNodes ()) {
                var i = 1;
                foreach (InputData input in node.GetInputs ()) {
                    if (_input.Guid == input.Guid) {
                        return new Rect (node.XPosition,
                            node.YPosition + (nodeConfig.TopMargin * 0.5f) + ((nodeConfig.InputSize) * i),
                            0,
                            0);
                    }
                    i++;
                }
            }
            return Rect.zero;
        }

        public Rect OutputPosition (OutputData _output) {
            foreach (NodeData node in constellationScript.GetNodes ()) {
                var j = 1;
                foreach (OutputData output in node.GetOutputs ()) {
                    if (_output.Guid == output.Guid) {
                        return new Rect (node.XPosition + nodeConfig.NodeWidth,
                            node.YPosition + (nodeConfig.TopMargin* 0.5f) + ((nodeConfig.InputSize ) * j),
                            0,
                            0);
                    }
                    j++;
                }
            }
            return Rect.zero;
        }

        public void DrawNodeCurve (Rect start, Rect end) {
            Vector3 startPos = new Vector3 (start.x + start.width, start.y + start.height / 2, 0);
            Vector3 endPos = new Vector3 (end.x, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Handles.DrawBezier (startPos, endPos, startTan, endTan, Color.Lerp (Color.grey, Color.yellow, 0.5f), null, 5);
        }
    }
}