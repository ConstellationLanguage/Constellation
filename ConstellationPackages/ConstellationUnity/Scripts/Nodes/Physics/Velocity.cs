using UnityEngine;

namespace Constellation.Physics {
    public class Velocity : INode, IReceiver, IRequireGameObject, IFixedUpdate, IUpdatable {
        private Rigidbody rigidBody;
        Vector3 force;
        ISender sender;
        Ray currentVelocity;
        bool isVelocityUpdated;
        public const string NAME = "Velocity";
        public void Setup (INodeParameters _nodeParameters) {
            _nodeParameters.AddInput (this, false, "Object", "Rigidbody affected");
            _nodeParameters.AddInput (this, false, "Vec3", "Vec3 world relative");
            sender = _nodeParameters.GetSender();
            _nodeParameters.AddOutput (true,"Vec3", "The current velocity of the rigidBody");

            force = Vector3.zero;
            Ray[] positions = new Ray[3];
            positions[0] = new Ray ().Set (0);
            positions[1] = new Ray ().Set (0);
            positions[2] = new Ray ().Set (0);
            currentVelocity = new Ray ();
            currentVelocity.Set (positions);
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
            if (!isVelocityUpdated){
                rigidBody.velocity = force;
                isVelocityUpdated = true;
            }
            force = Vector3.zero;
        }

        public void OnUpdate () {
            currentVelocity.SetAtIndex (rigidBody.velocity.x, 0);
            currentVelocity.SetAtIndex (rigidBody.velocity.y, 1);
            currentVelocity.SetAtIndex (rigidBody.velocity.z, 2);
            sender.Send (currentVelocity, 0);
        }

        public void Receive (Ray value, Input _input) {
            if (_input.InputId == 0)
                rigidBody = UnityObjectsConvertions.ConvertToGameObject (value.GetObject ()).GetComponent<UnityEngine.Rigidbody> () as UnityEngine.Rigidbody;

            if (_input.InputId == 1) {
                isVelocityUpdated = false;
                force = UnityObjectsConvertions.ConvertToVector3 (value);
            }
        }
    }
}