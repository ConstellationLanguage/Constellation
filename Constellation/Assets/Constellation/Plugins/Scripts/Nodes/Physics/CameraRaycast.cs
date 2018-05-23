using UnityEngine;

namespace Constellation.Physics {
    public class CameraRaycast : INode, IReceiver {
        private Vector3 movingVector;
        private ISender Sender;
        private ISender ObjectHit;
        private Vector3 hitPosition;

        private Variable valueX;
        private Variable valueY;
        private Variable Result;

        public const string NAME = "CameraRaycast";

        public void Setup (INodeParameters _nodeParameters) {
            _nodeParameters.AddInput (this, false, "Position X");
            _nodeParameters.AddInput (this, false, "Position Y");
            _nodeParameters.AddInput (this, true, "Calculate");
            Sender = _nodeParameters.GetSender();
            _nodeParameters.AddOutput (false,"Object", "The object that was hit");
           _nodeParameters.AddOutput (false, "The hit position");

            valueX = new Variable ().Set (0);
            valueY = new Variable ().Set (0);

        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void Receive (Variable value, Input _input) {
            if (_input.InputId == 0)
                valueX.Set (value.GetFloat ());

            if (_input.InputId == 1)
                valueY.Set (value.GetFloat ());

            if (_input.InputId == 2) {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay (new Vector2 (valueX.GetFloat (), valueY.GetFloat ()));

                if (UnityEngine.Physics.Raycast (ray, out hit)) {
                    Variable[] newVar = new Variable[3];
                    newVar[0] = new Variable ().Set (hit.point.x);
                    newVar[1] = new Variable ().Set (hit.point.y);
                    newVar[2] = new Variable ().Set (hit.point.z);
                    Result = new Variable ().Set (newVar);
                    Sender.Send(new Variable(hit.transform.gameObject),0);
                    Sender.Send (Result, 1);
                }

            }
        }
    }
}