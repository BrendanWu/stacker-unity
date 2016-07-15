using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	public TheStack theStack;

	private const float MOVING_SPEED = 5.0f;
	private const float STARTING_Y = 10.00f;

	private Vector3 desiredPosition;

	// Use this for initialization
	void Start ()
    {
        desiredPosition = transform.position;
        desiredPosition.y = theStack.getScoreCount() + STARTING_Y;
    }



    // Update is called once per frame
    void Update () {
		desiredPosition.y = theStack.getScoreCount() + STARTING_Y;

		transform.position = Vector3.Lerp(transform.position, desiredPosition, MOVING_SPEED * Time.deltaTime);
	}
}