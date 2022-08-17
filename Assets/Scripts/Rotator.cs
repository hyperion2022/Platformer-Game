using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour
{
	[SerializeField] float degrees = 90;

	// Before rendering each frame..
	void Update()
	{
		// Rotate the game object that this script is attached to by 45 in the Y axis,
		// multiplied by deltaTime in order to make it per second
		// rather than per frame.
		transform.Rotate(new Vector3(0, degrees, 0) * Time.deltaTime);
	}
}