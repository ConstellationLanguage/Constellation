using System.Collections;
using UnityEngine;
namespace OSCComponent {
	public class RotateAround : MonoBehaviour {

		Vector3 center;

		public float speed = 0.1f;

		// Use this for initialization
		void Start () {
			center = transform.position;
		}

		// Update is called once per frame
		void Update () {
			Vector3 newPosition = transform.position;

			newPosition.x = Mathf.Cos (Time.frameCount * speed) + center.x;
			newPosition.y = Mathf.Sin (Time.frameCount * speed) + center.y;
			transform.position = newPosition;

		}
	}
}