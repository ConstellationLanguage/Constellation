namespace Constellation.Unity {
	[System.Serializable]
	public class Update : INode, IUpdatable {
		public const string NAME = "Update";
		private ISender sender;
		private Ray value;

		public void Setup (INodeParameters _node) {
			sender = _node.GetSender();
			_node.AddOutput (true, "Ray", "Ray on update");
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

		public void OnUpdate () {
			sender.Send (value, 0);
		}
	}
}