using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour
{
	[SerializeField] float degrees = 45;

	// Before rendering each frame..
	void Update()
	{
		// Rotate the game object that this script is attached to by 45 in the Z axis,
		// multiplied by deltaTime in order to make it per second
		// rather than per frame.
		transform.Rotate(new Vector3(0, 0, degrees) * Time.deltaTime);
	}
}