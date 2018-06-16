using UnityEngine;

namespace Constellation.Unity {
	public class Quit : INode, IReceiver {
		public const string NAME = "Quit";
		public void Setup (INodeParameters _nodeParameters) {
			_nodeParameters.AddInput (this, false, "Quit application");
		}

		public string NodeName () {
			return NAME;
		}

		public string NodeNamespace () {
			return NameSpace.NAME;
		}

		public void Receive (Variable _value, Input _input) { 
			Application.Quit();
		}
	}
}