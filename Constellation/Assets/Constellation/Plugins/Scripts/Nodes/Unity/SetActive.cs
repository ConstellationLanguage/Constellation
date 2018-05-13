using UnityEngine;

namespace Constellation.Unity {
	public class SetActive : INode, IReceiver, IRequireGameObject {
		public const string NAME = "SetActive";
		UnityEngine.GameObject GameObject;
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
			GameObject = _gameObject;
		}

		public void Receive (Variable _value, Input _input) { 
			if(_input.isWarm){
				if(GameObject.activeSelf == true)
					sender.Send(new Variable(1), 0);
				else
					sender.Send(new Variable(0), 0);
			}

			if(_input.InputId == 0)
				GameObject = UnityObjectsConvertions.ConvertToGameObject(_value.GetObject());

			if(_input.InputId == 1){
				if(_value.GetFloat() == 1)
					GameObject.SetActive(true);
				else 
					GameObject.SetActive(false);
			}

		}
	}
}