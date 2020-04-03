using UnityEngine;

namespace Constellation.UI {
	public class Button : INode, IReceiver, IRequireGameObject, IDestroy {
		UnityEngine.UI.Button button;
		public const string NAME = "Button";
		private ISender output;

		public void Setup (INodeParameters _nodeParameters) {
			_nodeParameters.AddInput (this, false, "Object", "Button object");
			output = _nodeParameters.GetSender ();
			_nodeParameters.AddOutput (true, "Mouse Click");
		}

		public string NodeName () {
			return NAME;
		}

		public string NodeNamespace () {
			return NameSpace.NAME;
		}

		public void Set (GameObject _gameObject) {
			var button = _gameObject.GetComponent<UnityEngine.UI.Button> ();
			if (button != null) {
				this.button = button;
				this.button.onClick.AddListener(ButtonClicked);
			}
		}

		private void ButtonClicked()
		{
			output.Send(new Ray(button.gameObject.name), 0);
		}

		public void OnDestroy()
		{
			this.button.onClick.RemoveAllListeners();
			
		}

		public void Receive (Ray value, Input _input) {
			if (_input.InputId == 0)
				Set (UnityObjectsConvertions.ConvertToGameObject (value.GetObject ()));
		}
	}
}