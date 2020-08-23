namespace Constellation.UserInputs
{
    public class MousePosition : INode, IReceiver, IUpdatable {
        public const string NAME = "MousePosition";
        private ISender Sender;
        private ISender YPositionSender;
        private Ray XPosition;
        private Ray YPosition;
        private Ray keyState;
        public void Setup (INodeParameters _nodeParameters) {
            Sender = _nodeParameters.GetSender();
            _nodeParameters.AddOutput (true, "Mouse position X");
            _nodeParameters.AddOutput (true, "Mosue position Y");
            XPosition = new Ray(UnityEngine.Input.mousePosition.x);
            YPosition = new Ray(UnityEngine.Input.mousePosition.y);
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void OnUpdate () {
            Sender.Send (XPosition.Set (UnityEngine.Input.mousePosition.x), 0);
            Sender.Send (YPosition.Set (UnityEngine.Input.mousePosition.y), 1);
        }

        public void Receive (Ray value, Input _input) { }
    }
}