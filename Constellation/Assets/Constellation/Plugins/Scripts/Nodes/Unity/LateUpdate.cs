namespace Constellation.Unity {
	[System.Serializable]
	public class LateUpdate : INode, ILateUpdatable {
		public const string NAME = "LateUpdate";
		private ISender sender;
		private Variable value;

		public void Setup (INodeParameters _node) {
			sender = _node.GetSender();
			_node.AddOutput (true, "Ray on lateUpdate");
			value = new Variable ();
			value.Set ("Ray");
		}

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

		public void Receive (Variable _value, Input _input) {

		}

		public void OnLateUpdate () {
			sender.Send (value, 0);
		}
	}
}