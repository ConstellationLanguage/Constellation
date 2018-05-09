using UnityEngine;

namespace Constellation.Physics {
    public class AddImpulse : INode, IReceiver, IRequireGameObject, IFixedUpdate {
        public const string NAME = "AddImpulse";
        Rigidbody rigidBody;
        bool requestedImpulse = false;
        Vector3 force;
        public void Setup (INodeParameters _nodeParameters) {
            _nodeParameters.AddInput (this, false,"Vec3 world relative");
            force = Vector3.zero;
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Set (GameObject _gameObject) {
            rigidBody = _gameObject.GetComponent<Rigidbody> () as Rigidbody;
            if (rigidBody == null)
                rigidBody = _gameObject.AddComponent<Rigidbody> ();
        }

        public void OnFixedUpdate () {
            if(requestedImpulse){
                requestedImpulse = false;
                rigidBody.AddForce (force, ForceMode.Impulse);
            }
        }

        public void Receive (Variable value, Input _input) {
            if (_input.InputId == 0) {
                if(!requestedImpulse) {
                    force = UnityObjectsConvertions.ConvertToVector3 (value);
                    requestedImpulse = true;
                }
            }
        }
    }
}