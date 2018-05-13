using UnityEngine;

namespace Constellation.Unity {
    public class Transform : INode, IReceiver, IRequireGameObject {
        public const string NAME = "Transform";
        private Transform transform;
        private GameObject gameObject;
        private Variable GameObject;
        private Variable Position;
        private Variable Rotation;
        private Variable Scale;
        private Variable Name;
        private ISender sender;

        private Rigidbody rigidBody;

        public void Setup (INodeParameters _nodeParameters) {
            _nodeParameters.AddInput (this, false, "Object", "Transform object");
            _nodeParameters.AddInput (this, false, "Vec3 position");
            _nodeParameters.AddInput (this, false, "Vec3 rotation");
            _nodeParameters.AddInput (this, false, "Vec3 scale");
            _nodeParameters.AddInput (this, true, "Send");

             sender =  _nodeParameters.GetSender();
            _nodeParameters.AddOutput (false, "Vec3 position");
            _nodeParameters.AddOutput (false, "Vec3 rotation");
            _nodeParameters.AddOutput (false, "Vec3 scale");
            _nodeParameters.AddOutput(false, "Object", "Transform");

            GameObject = new Variable ().Set (null as object);
            Variable[] newPositionVar = new Variable[3];
            newPositionVar[0] = new Variable ().Set (0);
            newPositionVar[1] = new Variable ().Set (0);
            newPositionVar[2] = new Variable ().Set (0);
            Position = new Variable ().Set (newPositionVar);
            Variable[] newRotationVar = new Variable[3];
            
            newRotationVar[0] = new Variable ().Set (0);
            newRotationVar[1] = new Variable ().Set (0);
            newRotationVar[2] = new Variable ().Set (0);
            Rotation = new Variable ().Set (newRotationVar);

            Variable[] newScaleVar = new Variable[3];
            newScaleVar[0] = new Variable ().Set (0);
            newScaleVar[1] = new Variable ().Set (0);
            newScaleVar[2] = new Variable ().Set (0);
            Scale = new Variable ().Set (newScaleVar);
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

        public void Receive (Variable value, Input _input) {
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

            if (_input.isWarm) {
                UpdateTransform ();
                sender.Send (Position, 0);
                sender.Send (Rotation, 1);
                sender.Send (Scale, 2);
                sender.Send(GameObject, 3);
            }
        }
    }
}