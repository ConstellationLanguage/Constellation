using UnityEngine;

namespace Constellation.Unity {
	public class Quit : INode, IReceiver {
		public const string NAME = "Quit";
		UnityEngine.GameObject GameObject;
		ISender sender;
		public void Setup (INodeParameters _nodeParameters) {
			_nodeParameters.AddInput (this, false, "Quit application");
			sender = _nodeParameters.GetSender();
		}

		public string NodeName () {
			return NAME;
		}

		public string NodeNamespace () {
			return NameSpace.NAME;
		}

		public void Set (GameObject _gameObject) {
			GameObject = _gameObject;
		}

		public void Receive (Variable _value, Input _input) { 
			Application.Quit();
		}
	}
}