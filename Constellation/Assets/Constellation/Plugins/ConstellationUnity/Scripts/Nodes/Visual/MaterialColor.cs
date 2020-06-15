using UnityEngine;

namespace Constellation.Visual {
    public class MaterialColor : INode, IReceiver, IAwakable, IRequireGameObject {
        public const string NAME = "MaterialColor";
        private Renderer renderer;
        private string parameterName;
        private UnityEngine.Color color;

        public void Setup (INodeParameters _nodeParameters) {
            _nodeParameters.AddInput (this, false, "Object", "Game Object");
            _nodeParameters.AddInput (this, false, "Color", "Color");
            color = UnityEngine.Color.black;
        }

        public string NodeName () {
            return NAME;
        }

        public void Set (GameObject gameObject) {
            var _renderer = gameObject.GetComponent<Renderer> ();
            if (_renderer != null)
                renderer = _renderer;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void OnAwake () {

        }

        public void Receive (Ray value, Input _input) {

            if (_input.InputId == 0) {
                var gameObject = UnityObjectsConvertions.ConvertToGameObject (value.GetObject ());
                if (gameObject != null)
                    renderer = gameObject.GetComponent<Renderer> ();
            }

            if (renderer == null) {
                return;
            }

            if (_input.InputId == 1) {
                color = new UnityEngine.Color (value.GetArrayVariable (0).GetFloat(), value.GetArrayVariable (1).GetFloat(), value.GetArrayVariable (2).GetFloat(), value.GetArrayVariable (3).GetFloat());
                renderer.material.color = color;
            }
        }
    }
}