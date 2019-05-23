using Constellation;
using UnityEngine;

namespace ConstellationEditor {
    public class NodeEditorLinks : ILinkEditor {
        private InputData selectedInput;
        private OutputData selectedOutput;
        private ConstellationScript constellationScript;
        private bool isInstance;
        private LinkView LinksView;
        private IGUI GUI;
        public delegate void LinkAdded (LinkData link);
        LinkAdded OnLinkAdded;
        public delegate void LinkRemoved (LinkData link);
        LinkRemoved OnLinkRemoved;
        IUndoable undoable;

        public NodeEditorLinks (ConstellationScript _constellationScript,
            bool _isInstance,
            IGUI _gui,
            NodeConfig _nodeConfig,
            LinkAdded _onLinkAdded, LinkRemoved _onLinkRemoved,
            NodeEditorPanel _nodeEditorPannel,
            IUndoable _undoable) {

            OnLinkAdded += _onLinkAdded;
            OnLinkRemoved += _onLinkRemoved;
            undoable = _undoable;
            constellationScript = _constellationScript;
            isInstance = _isInstance;
            GUI = _gui;
            LinksView = new LinkView (GUI, _nodeEditorPannel, constellationScript, _nodeConfig, linkRemoved);
        }

        public LinkData[] GetLinks () {
            return LinksView.GetLinks ();
        }

        public void DrawLinks () {
            LinksView.DrawLinks ();
            DrawIncompleteLink ();

        }

        public void AddLinkFromOutput (OutputData _output) {
            if (selectedInput != null)
                CreateLink (selectedInput, _output);
            else if (selectedOutput == null)
                selectedOutput = _output;

        }

        public void AddLinkFromInput (InputData _input) {
            if (selectedOutput != null) 
                CreateLink (_input, selectedOutput);
            else if (selectedInput == null)
                selectedInput = _input;
        }

        public void CreateLink (InputData _input, OutputData _output) {
            if (isInstance)
                constellationScript.IsDifferentThanSource = true;

            selectedInput = null;
            selectedOutput = null;
            var newLink = new LinkData (_input, _output);
            if (constellationScript.IsLinkValid (newLink)) {
                constellationScript.AddLink (newLink);
                OnLinkAdded (newLink);
                undoable.AddAction ();
                GUI.RequestRepaint ();
            }
        }

        private void DrawIncompleteLink () {
            if (selectedInput != null || selectedOutput != null) {
                var e = Event.current;
                if (selectedInput != null) {
                    LinksView.DrawNodeCurve (new Rect (e.mousePosition.x, e.mousePosition.y, 0, 0), LinksView.InputPosition (selectedInput));
                    GUI.RequestRepaint ();
                } else if (selectedOutput != null) {
                    LinksView.DrawNodeCurve (LinksView.OutputPosition (selectedOutput), new Rect (e.mousePosition.x, e.mousePosition.y, 0, 0));
                    GUI.RequestRepaint ();
                }

                if (e.button == 1) {
                    selectedInput = null;
                    selectedOutput = null;
                }
            }
        }

        public void linkRemoved (LinkData link) {
            OnLinkRemoved (link);
        }
    }
}