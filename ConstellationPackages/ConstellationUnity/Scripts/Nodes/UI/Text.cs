using UnityEngine;

namespace Constellation.UI {
	public class Text : INode, IReceiver, IRequireGameObject {
		UnityEngine.UI.Text text;
		public const string NAME = "Text";

		public void Setup (INodeParameters _nodeParameters) {
			_nodeParameters.AddInput (this, false, "Object", "Text object");
			_nodeParameters.AddInput (this, false, "Text to pass");
		}

		public string NodeName () {
			return NAME;
		}

		public string NodeNamespace () {
			return NameSpace.NAME;
		}

		public void Set (GameObject _gameObject) {
			var body = _gameObject.GetComponent<UnityEngine.UI.Text> ();
			if (body != null)
				text = body;
		}

		public void Receive (Ray value, Input _input) {
			if (_input.InputId == 0)
                Set(UnityObjectsConvertions.ConvertToGameObject (value.GetObject ()));
			if (_input.InputId == 1)
				text.text = value.GetString();
		}
	}
}