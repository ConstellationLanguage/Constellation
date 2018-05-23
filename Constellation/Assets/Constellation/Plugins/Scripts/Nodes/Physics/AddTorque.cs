using UnityEngine;

namespace Constellation.Physics {
    public class AddTorque : INode, IReceiver, IRequireGameObject, IFixedUpdate {
        Rigidbody rigidBody;
        Vector3 torque;
        public const string NAME = "AddTorque";
        public void Setup (INodeParameters _nodeParameters) {
            _nodeParameters.AddInput (this, false, "Vec3 World relative");
            torque = Vector3.zero;
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
            if(torque == Vector3.zero)
                return;
            rigidBody.AddTorque (torque * Time.fixedDeltaTime);
            torque = Vector3.zero;
        }

        public void Receive (Variable value, Input _input) {
            if (_input.InputId == 0) {
                torque = UnityObjectsConvertions.ConvertToVector3 (value);
            }
        }
    }
}