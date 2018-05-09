using UnityEngine;

namespace Constellation.Physics {
    public class RigidBody : INode, IReceiver, IRequireGameObject {
        Rigidbody rigidBody;
        private Variable Mass;
        private Variable Drag;
        private Variable UseGravity;
        private Variable IsKinematic;
        private Variable PositionConstraints;
        private Variable RotationConstraints;
        public const string NAME = "RigidBody";

        public void Setup (INodeParameters _nodeParameters) {
            _nodeParameters.AddInput (this, false, "Object", "Rigidbody object");
            _nodeParameters.AddInput (this, false, "Mass");
            _nodeParameters.AddInput (this, false, "Drag");
            _nodeParameters.AddInput (this, false, "Angular drag");
            _nodeParameters.AddInput (this, false, "Use gravity (1/0)");
            _nodeParameters.AddInput (this, false, "Is kinematic (1/0)");
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Set (GameObject _gameObject) {
            var body = _gameObject.GetComponent<UnityEngine.Rigidbody> ();
            if (body != null)
                rigidBody = body;
        }

        public void Receive (Variable value, Input _input) {
            if (_input.InputId == 0)
                Set(UnityObjectsConvertions.ConvertToGameObject (value.GetObject ()));
            else if (_input.InputId == 1)
                rigidBody.mass = value.GetFloat ();
            else if (_input.InputId == 2)
                rigidBody.drag = value.GetFloat ();
            else if (_input.InputId == 3)
                rigidBody.angularDrag = value.GetFloat ();
            else if (_input.InputId == 4) {
                if (value.GetFloat () == 1)
                    rigidBody.useGravity = true;
                else
                    rigidBody.useGravity = false;
            } else if (_input.InputId == 5) {
                if (value.GetFloat () == 1)
                    rigidBody.isKinematic = true;
                else
                    rigidBody.isKinematic = false;
            }
        }
    }
}