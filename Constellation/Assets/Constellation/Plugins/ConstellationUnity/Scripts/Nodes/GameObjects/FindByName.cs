namespace Constellation.GameObjects {
    public class FindByName : INode, IReceiver, IAwakable {
        public const string NAME = "FindByName";
        private ISender sender;
        public Ray GameObject;

        public void Setup (INodeParameters _nodeParameters) {
            sender = _nodeParameters.GetSender ();
            _nodeParameters.AddInput (this, true, "Game Object Name");
            _nodeParameters.AddOutput (false, "Object", "Gameobject");
        }

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

        public void OnAwake () {

        }

        public void Receive (Ray value, Input _input) {
            if (_input.isBright){
                GameObject = new Ray ().Set (UnityEngine.GameObject.Find (value.GetString ()));
                sender.Send (GameObject, 0);
            }
        }
    }
}