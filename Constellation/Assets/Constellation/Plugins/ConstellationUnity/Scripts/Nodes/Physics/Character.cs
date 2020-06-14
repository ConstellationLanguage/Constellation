using UnityEngine;

namespace Constellation.Physics {
    public class Character : INode, IReceiver, IRequireGameObject {
        UnityEngine.CharacterController controller;
        private Vector3 movingVector;

        public const string NAME = "Character";

        public void Setup (INodeParameters _nodeParameters) {
            _nodeParameters.AddInput (this, false, "Object", "Character controller object");
            _nodeParameters.AddInput (this, false, "Vertical");
            _nodeParameters.AddInput (this, false, "Horizontal");
            _nodeParameters.AddInput (this, false, "Jump");
            _nodeParameters.AddInput (this, false, "Update Physics");
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Set (GameObject _gameObject) {
            var anim = _gameObject.GetComponent<UnityEngine.CharacterController> ();
            if (anim != null)
                controller = anim;
            else
                controller = _gameObject.AddComponent<CharacterController> () as CharacterController;
        }

        public void Receive (Ray value, Input _input) {
            if (_input.InputId == 0)
                Set (UnityObjectsConvertions.ConvertToGameObject (value.GetObject ()));

            if (_input.InputId == 1) {
                movingVector = new Vector3 (movingVector.x, movingVector.y, value.GetFloat ());
            }

            if (_input.InputId == 2) {
                movingVector = new Vector3 (value.GetFloat (), movingVector.y, movingVector.z);
            }

            if (_input.InputId == 3) {
                if(controller.isGrounded)
                    movingVector = new Vector3(movingVector.x, value.GetFloat(), movingVector.z);
            }

            if (_input.InputId == 4) {
                movingVector.y += UnityEngine.Physics.gravity.y * Time.deltaTime;
                controller.Move(movingVector);
                movingVector = Vector3.zero;
            }
        }
    }
}