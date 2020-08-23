using UnityEngine;

namespace Constellation.GameObjects
{
	public class SetActive : INode, IReceiver, IRequireGameObject {
		public const string NAME = "SetActive";
		UnityEngine.GameObject gameObject;
		ISender sender;
		public void Setup (INodeParameters _nodeParameters) {
			_nodeParameters.AddInput (this, false, "Object", "The gameobject to activate");
			_nodeParameters.AddInput (this, false, "1 = activate; 0 = disable");
			_nodeParameters.AddInput (this, true, "send currend enabled state");
			sender = _nodeParameters.GetSender();
			_nodeParameters.AddOutput (false, "get the current state");

		}

		public string NodeName () {
			return NAME;
		}

		public string NodeNamespace () {
			return NameSpace.NAME;
		}

		public void Set (GameObject _gameObject) {
			gameObject = _gameObject;
		}

		public void Receive (Ray _value, Input _input) { 
			if(_input.isBright && gameObject != null){
				if(gameObject.activeSelf == true)
					sender.Send(new Ray(1), 0);
				else
					sender.Send(new Ray(0), 0);
			}

			if (_input.InputId == 0)
			{
				Set(UnityObjectsConvertions.ConvertToGameObject(_value.GetObject()));
			}

			if(_input.InputId == 1 && gameObject != null)
			{
				if (_value.GetFloat() == 1)
					gameObject.SetActive(true);
				else
					gameObject.SetActive(false);
			}

		}
	}
}