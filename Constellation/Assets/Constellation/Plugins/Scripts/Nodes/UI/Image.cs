using UnityEngine;

namespace Constellation.UI {
	public class Image : INode, IReceiver, IRequireGameObject {
		UnityEngine.UI.Image image;
		public const string NAME = "Image";
		private Variable ColorVar;

		public void Setup (INodeParameters _nodeParameters) {
			_nodeParameters.AddInput (this, false, "Object", "Button object");
			_nodeParameters.AddInput (this, false, "Object", "Image");
			_nodeParameters.AddInput (this, false, "Color");

			Variable[] newColorVar = new Variable[4];
			newColorVar[0] = new Variable ().Set (0);
			newColorVar[1] = new Variable ().Set (0);
			newColorVar[2] = new Variable ().Set (0);
			newColorVar[3] = new Variable ().Set (0);
			ColorVar = new Variable ().Set (newColorVar);
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
			var image = _gameObject.GetComponent<UnityEngine.UI.Image> ();
			if (image != null) {
				this.image = image;
			}
		}

		public void Receive (Variable value, Input _input) {
			if (_input.InputId == 0)
				Set (UnityObjectsConvertions.ConvertToGameObject (value.GetObject ()));

			if(_input.InputId == 1){
				var sprite = UnityObjectsConvertions.ConvertToSprite(value);
				if(sprite != null){
					image.sprite = sprite;
				}
			}

			if (_input.InputId == 2) {
				ColorVar.Set (value.GetArray ());
				image.color = new Color (ColorVar.GetArrayVariable (0).GetFloat () * 0.01f, ColorVar.GetArrayVariable (1).GetFloat ()* 0.01f, ColorVar.GetArrayVariable (2).GetFloat ()* 0.01f, ColorVar.GetArrayVariable (3).GetFloat ()* 0.01f);
			}
		}
	}
}