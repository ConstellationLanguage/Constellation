using UnityEngine;
using uDesktopDuplication;
namespace Constellation.Experimental {
    public class DesktopCapture : INode, IReceiver, IGameObject {
        private ISender sender;
        private Attribute attribute; // attributes are setted in the editor.
        public const string NAME = "DesktopCapture"; //Setting the node name (need to be a const to be used in the factory without the node instantiated)
        private GameObject GameObject;
        private uDesktopDuplication.Texture uDesktopTexture;

        public void Setup (INodeParameters _node, ILogger _logger) {
            sender = _node.GetSender();
            _node.AddInput(this, false, "Object", "uDesktop texture gameobject");
            _node.AddInput(this, false, "Set monitor");
            _node.AddInput(this, true, "Get current monitor");
            _node.AddOutput(false, "Monitor id");
        }

        public void Set (GameObject gameObject) {
            GameObject = gameObject;
            var texture = GameObject.GetComponent<uDesktopDuplication.Texture> ();
            if (texture == null) {
                texture = GameObject.AddComponent<uDesktopDuplication.Texture> ();
            }
            uDesktopTexture = texture;
        }
        //return the node name (used in the factory).
        public string NodeName () {
            return NAME;
        }

        private void SetMonitor (int wantedID) {
            var texture = uDesktopTexture;
            var id = texture.monitorId;
            var n = uDesktopDuplication.Manager.monitorCount;
            if (wantedID >= 0 && wantedID < n)
                texture.monitorId = wantedID;
        }

        //return the node namespace (used for the factory)
        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        //Receive from inputs.
        public void Receive (Variable _value, Input _input) {
            if(_input.InputId == 0)
                Set(UnityObjectsConvertions.ConvertToGameObject(_value.GetObject()));
            
            if(_input.InputId == 1)
                SetMonitor(Mathf.FloorToInt(_value.GetFloat()));

        }
    }
}