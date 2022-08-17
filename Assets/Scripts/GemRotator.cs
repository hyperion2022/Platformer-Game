using UnityEngine;
using System.Collections;

public class GemRotator : MonoBehaviour
{
	[SerializeField] float degrees = 180;

	// Before rendering each frame..
	void Update()
	{
		// Rotator script adapted to the position of Gem Prefabs
		transform.Rotate(new Vector3(0, 0, degrees) * Time.deltaTime);
	}
}