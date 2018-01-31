using UnityEngine;

namespace Constellation.Unity {
	public class LookAtPosition : INode, IReceiver, IGameObject {

		public const string NAME = "LookAtPosition";
		private GameObject gameobject;
		private Vector3 gameobjectPosition;
		private Vector3 targetPosition;
		private Variable ResultRotation;
		private ISender outputRotation;

		public void Setup (INodeParameters _nodeParameters, ILogger _logger) {
			_nodeParameters.AddInput (this, false, "GameObject position");
			_nodeParameters.AddInput (this, true, "Position to look at");
			outputRotation = _nodeParameters.AddOutput (false, "Target rotation");
		}

		public void Set (GameObject _gameObject) {
			gameobject = _gameObject;

		}

		public string NodeName () {
			return NAME;
		}

		public string NodeNamespace () {
			return NameSpace.NAME;
		}

		public void Receive (Variable value, Input _input) {
			if (_input.InputId == 0) {
				if (value.GetObject () == null) {
					var vector = new Vector3 (value.GetFloat (0), value.GetFloat (1), value.GetFloat (2));
					gameobjectPosition = vector;
				} else if (value.GetObject () != null) {
					var vector = UnityObjectsConvertions.ConvertToVector3 (value.GetObject ());
					gameobjectPosition = vector;
				}
			}

			if (_input.InputId == 1) {
				if (value.GetObject () == null) {
					var vector = new Vector3 (value.GetFloat (0), value.GetFloat (1), value.GetFloat (2));
					targetPosition = vector;
				} else if (value.GetObject () != null) {
					var vector = UnityObjectsConvertions.ConvertToVector3 (value.GetObject ());
					targetPosition = vector;
				}
			}

			if (_input.isWarm) {
				var targetRotation = Quaternion.LookRotation (targetPosition - gameobjectPosition).eulerAngles;
				Variable[] newVar = new Variable[3];
				newVar[0] = new Variable(targetRotation.x);
				newVar[1] = new Variable(targetRotation.y);
				newVar[2] =new Variable(targetRotation.z);
				ResultRotation = new Variable ().Set (newVar);
				outputRotation.Send(ResultRotation, 0);
			}
		}
	}
}