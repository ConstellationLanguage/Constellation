namespace Constellation.Physics {
	[System.Serializable]
	public class FixedUpdate : INode, IFixedUpdate {
		public const string NAME = "FixedUpdate";
		private ISender sender;
		private Variable value;

		public void Setup (INodeParameters _node) {
			sender = _node.GetSender();
			_node.AddOutput (true, "Ray on update");
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

		public void OnFixedUpdate () {

			sender.Send (value, 0);
		}
	}
}