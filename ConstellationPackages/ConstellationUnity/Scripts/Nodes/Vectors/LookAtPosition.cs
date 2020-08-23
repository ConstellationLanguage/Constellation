using UnityEngine;

namespace Constellation.Vectors {
	public class LookAtPosition : INode, IReceiver{

		public const string NAME = "LookAtPosition";
		private Vector3 gameobjectPosition;
		private Vector3 targetPosition;
		private Ray ResultRotation;
		private ISender sender;

		public void Setup (INodeParameters _nodeParameters) {
			_nodeParameters.AddInput (this, false,"Object", "GameObject position");
			_nodeParameters.AddInput (this, true, "Vec3", "Position to look at");
			sender = _nodeParameters.GetSender();
			_nodeParameters.AddOutput (false,"Vec3", "Target rotation");
		}

		public string NodeName () {
			return NAME;
		}

		public string NodeNamespace () {
			return NameSpace.NAME;
		}

		public void Receive (Ray value, Input _input) {
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

			if (_input.isBright) {
				var targetRotation = Quaternion.LookRotation (targetPosition - gameobjectPosition).eulerAngles;
				Ray[] newVar = new Ray[3];
				newVar[0] = new Ray(targetRotation.x);
				newVar[1] = new Ray(targetRotation.y);
				newVar[2] =new Ray(targetRotation.z);
				ResultRotation = new Ray ().Set (newVar);
				sender.Send(ResultRotation, 0);
			}
		}
	}
}