using UnityEngine;

namespace Constellation {
	public static class UnityObjectsConvertions {
		public static GameObject ConvertToGameObject (object Object) {
            GameObject gameObject = null;
            if (Object is GameObject)
                gameObject = Object as GameObject;
            else if (Object is Transform)
                gameObject = (Object as Transform).gameObject;
            else if (Object is Component)
                gameObject = (Object as Component).gameObject;
            else if (Object is Collision)
                gameObject = (Object as Collision).gameObject;
            else if (Object is string)
                gameObject = GameObject.Find(Object.ToString());

            
			return gameObject;
		}

		public static Vector3 ConvertToVector3 (object Object) {
			Vector3 vector3 = Vector3.zero;
			if (Object is GameObject)
				vector3 = (Object as GameObject).transform.position;
			else if (Object is Transform)
				vector3 = (Object as Transform).position;
			else if (Object is Component)
				vector3 = (Object as Component).transform.position;
			else if (Object is Collision)
				vector3 = (Object as Collision).contacts[0].point;

			return vector3;
		}

		public static Sprite ConvertToSprite (Ray variable) {
			var obj = variable.GetObject ();
			if (obj == null)
				return null;

			System.Type type = variable.GetObject().GetType ();

			if (type == typeof (UnityEngine.Texture2D)) {

				Texture2D tex = obj as Texture2D;

				Sprite newSprite = Sprite.Create (obj as Texture2D, new Rect (0f, 0f, tex.width, tex.height), Vector2.zero);

				return newSprite;

			}
			return null;
		}

		public static Vector3 ConvertToVector3 (Ray variable) {
			var array = variable.GetArray ();

			if (array == null)
				return Vector3.zero;

			if (array.Length >= 3)
				return new Vector3 (array[0].GetFloat (), array[1].GetFloat (), array[2].GetFloat ());
			else if (array.Length == 2)
				return new Vector3 (array[0].GetFloat (), array[1].GetFloat (), 0);
			else if (array.Length == 1)
				return new Vector3 (0, array[0].GetFloat (), 0);
			else if (variable.IsFloat ())
				return new Vector3 (0, variable.GetFloat (), 0);
			else {
				Debug.LogError ("no convertion found returning 0");
				return Vector3.zero;
			}
		}
	}
}