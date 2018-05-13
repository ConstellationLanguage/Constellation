using UnityEngine;

namespace Constellation.UI {
	public class Slider : INode, IReceiver, IRequireGameObject {
		UnityEngine.UI.Slider slider;
		public const string NAME = "Slider";
		public ISender sender;
		private Variable sliderValue;
		private GameObject gameObject;

		public void Setup (INodeParameters _nodeParameters) {
			_nodeParameters.AddInput (this, false, "Object", "Slider object");
			_nodeParameters.AddInput (this, false, "Slider value");
			_nodeParameters.AddInput (this, true, "Push");
			_nodeParameters.AddOutput (false, "Slider Value");
			sender = _nodeParameters.GetSender ();
			sliderValue = new Variable (0);
		}

		void UpdateImage () {

		}

		public string NodeName () {
			return NAME;
		}

		public string NodeNamespace () {
			return NameSpace.NAME;
		}

		public void Set (GameObject _gameObject) {
			gameObject = _gameObject;
			var slider = _gameObject.GetComponent<UnityEngine.UI.Slider> ();
			if (slider == null) {
				AddSlider();
			} else
				this.slider = slider;
		}

		private void AddSlider()
		{
			slider = gameObject.AddComponent<UnityEngine.UI.Slider>();
		}

		public void Receive (Variable value, Input _input) {
			if (_input.InputId == 0)
				Set (UnityObjectsConvertions.ConvertToGameObject (value.GetObject ()));

			if (_input.InputId == 1) {
				if(slider == null)
					AddSlider();
				slider.value = value.GetFloat ();
			}

			if (_input.isWarm) {
				sliderValue = new Variable (slider.value);
				sender.Send (sliderValue, 0);
			}
		}
	}
}