using UnityEngine;

namespace Constellation.GameObjects {
    public class Transform : INode, IReceiver, IRequireGameObject {
        public const string NAME = "Transform";
        private GameObject gameObject;
        private Ray GameObject;
        private Ray Position;
        private Ray Rotation;
        private Ray Scale;
        private Ray Name;
        private ISender sender;

        private Rigidbody rigidBody;

        public void Setup (INodeParameters _nodeParameters) {
            _nodeParameters.AddInput (this, false, "Object", "Transform object");
            _nodeParameters.AddInput (this, false, "Vec3", "Vec3 position");
            _nodeParameters.AddInput (this, false, "Vec3", "Vec3 rotation");
            _nodeParameters.AddInput (this, false, "Vec3", "Vec3 scale");
            _nodeParameters.AddInput (this, true, "Any", "Send");

             sender =  _nodeParameters.GetSender();
            _nodeParameters.AddOutput (false, "Vec3", "Vec3 position");
            _nodeParameters.AddOutput (false, "Vec3", "Vec3 rotation");
            _nodeParameters.AddOutput (false, "Vec3", "Vec3 scale");
            _nodeParameters.AddOutput(false, "Object", "Transform");

            GameObject = new Ray ().Set (null as object);
            Ray[] newPositionVar = new Ray[3];
            newPositionVar[0] = new Ray ().Set (0);
            newPositionVar[1] = new Ray ().Set (0);
            newPositionVar[2] = new Ray ().Set (0);
            Position = new Ray ().Set (newPositionVar);
            Ray[] newRotationVar = new Ray[3];
            
            newRotationVar[0] = new Ray ().Set (0);
            newRotationVar[1] = new Ray ().Set (0);
            newRotationVar[2] = new Ray ().Set (0);
            Rotation = new Ray ().Set (newRotationVar);

            Ray[] newScaleVar = new Ray[3];
            newScaleVar[0] = new Ray ().Set (0);
            newScaleVar[1] = new Ray ().Set (0);
            newScaleVar[2] = new Ray ().Set (0);
            Scale = new Ray ().Set (newScaleVar);
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Set (GameObject _gameObject) {
            gameObject = _gameObject;
            GameObject.Set (gameObject);
            UpdateTransform ();
            rigidBody = gameObject.GetComponent<Rigidbody>();
        }

        void UpdateTransform () {
            gameObject = UnityObjectsConvertions.ConvertToGameObject (GameObject.GetObject ()) as GameObject;
            Position.SetAtIndex (gameObject.transform.localPosition.x, 0);
            Position.SetAtIndex (gameObject.transform.localPosition.y, 1);
            Position.SetAtIndex (gameObject.transform.localPosition.z, 2);

            Rotation.SetAtIndex (gameObject.transform.rotation.eulerAngles.x, 0);
            Rotation.SetAtIndex (gameObject.transform.rotation.eulerAngles.y, 1);
            Rotation.SetAtIndex (gameObject.transform.rotation.eulerAngles.z, 2);

            Scale.SetAtIndex (gameObject.transform.localScale.x, 0);
            Scale.SetAtIndex (gameObject.transform.localScale.y, 1);
            Scale.SetAtIndex (gameObject.transform.localScale.z, 2);
        }

        public void Receive (Ray value, Input _input) {
            if (_input.InputId == 1) {
                Position.Set (value.GetArray ());
                if(rigidBody == null || gameObject.activeInHierarchy == false)
                    gameObject.transform.localPosition = new Vector3 (Position.GetArrayVariable (0).GetFloat (), Position.GetArrayVariable (1).GetFloat (), Position.GetArrayVariable (2).GetFloat ());
                else 
                    rigidBody.position = new Vector3 (Position.GetArrayVariable (0).GetFloat (), Position.GetArrayVariable (1).GetFloat (), Position.GetArrayVariable (2).GetFloat ());
            } else if (_input.InputId == 2) {
                Rotation.Set (value.GetArray ());
                if(rigidBody == null)
                    gameObject.transform.rotation = Quaternion.Euler (new Vector3 (Rotation.GetArrayVariable (0).GetFloat (), Rotation.GetArrayVariable (1).GetFloat (), Rotation.GetArrayVariable (2).GetFloat ()));
                else 
                    rigidBody.rotation = Quaternion.Euler (new Vector3 (Rotation.GetArrayVariable (0).GetFloat (), Rotation.GetArrayVariable (1).GetFloat (), Rotation.GetArrayVariable (2).GetFloat ()));
            } else if (_input.InputId == 3) {
                Scale.Set (value.GetArray ());
                gameObject.transform.localScale = new Vector3 (Scale.GetArrayVariable (0).GetFloat (), Scale.GetArrayVariable (1).GetFloat (), Scale.GetArrayVariable (2).GetFloat ());
            } else if (_input.InputId == 0) {
                var obj = UnityObjectsConvertions.ConvertToGameObject (value.GetObject ());
                if (obj is GameObject) {
                    Set (obj);
                }
            }

            if (_input.isBright) {
                UpdateTransform ();
                sender.Send (Position, 0);
                sender.Send (Rotation, 1);
                sender.Send (Scale, 2);
                sender.Send(GameObject, 3);
            }
        }
    }
}