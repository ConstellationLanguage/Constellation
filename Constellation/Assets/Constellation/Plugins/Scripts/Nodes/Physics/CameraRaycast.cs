using UnityEngine;

namespace Constellation.Physics {
    public class CameraRaycast : INode, IReceiver {
        private Vector3 movingVector;
        private ISender Sender;
        private ISender ObjectHit;
        private Vector3 hitPosition;

        private Ray valueX;
        private Ray valueY;
        private Ray Result;

        public const string NAME = "CameraRaycast";

        public void Setup (INodeParameters _nodeParameters) {
            _nodeParameters.AddInput (this, false, "Position X");
            _nodeParameters.AddInput (this, false, "Position Y");
            _nodeParameters.AddInput (this, true, "Any", "Calculate");
            Sender = _nodeParameters.GetSender();
            _nodeParameters.AddOutput (false,"Object", "The object that was hit");
           _nodeParameters.AddOutput (false, "Vec3", "The hit position");

            valueX = new Ray ().Set (0);
            valueY = new Ray ().Set (0);

        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Ray value, Input _input) {
            if (_input.InputId == 0)
                valueX.Set (value.GetFloat ());

            if (_input.InputId == 1)
                valueY.Set (value.GetFloat ());

            if (_input.InputId == 2) {
                RaycastHit hit;
                UnityEngine.Ray ray = Camera.main.ScreenPointToRay (new Vector2(valueX.GetFloat (), valueY.GetFloat ()));

                if (UnityEngine.Physics.Raycast (ray, out hit)) {
                    Ray[] newVar = new Ray[3];
                    newVar[0] = new Ray ().Set (hit.point.x);
                    newVar[1] = new Ray ().Set (hit.point.y);
                    newVar[2] = new Ray ().Set (hit.point.z);
                    Result = new Ray ().Set (newVar);
                    Sender.Send(new Ray(hit.transform.gameObject),0);
                    Sender.Send (Result, 1);
                }

            }
        }
    }
}