namespace Constellation.Unity {
	[System.Serializable]
	public class LateUpdate : INode, ILateUpdatable {
		public const string NAME = "LateUpdate";
		private ISender sender;
		private Ray value;

		public void Setup (INodeParameters _node) {
			sender = _node.GetSender();
			_node.AddOutput (true, "Ray", "Ray on lateUpdate");
			value = new Ray ();
			value.Set ("Ray");
		}

        public string NodeName () {
            return NAME;
        }

        public string NodeNamespace () {
            return NameSpace.NAME;
        }

		public void Receive (Ray _value, Input _input) {

		}

		public void OnLateUpdate () {
			sender.Send (value, 0);
		}
	}
}