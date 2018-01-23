using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Direction
{
	North, 
	South, 
	East, 
	West
};
public class Player : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (0.5f, 0.5f, 0.0f);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.LeftArrow)) 
		{
			Move (Direction.West);
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) 
		{
			Move (Direction.East);
		}
		if (Input.GetKeyDown (KeyCode.UpArrow)) 
		{
			Move (Direction.North);
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) 
		{
			Move (Direction.South);
		}
	}

	private void Move(Direction direction)
	{
		Vector3 move = new Vector3 ();
		if (direction == Direction.North) {
			move = new Vector3 (0, 1, 0);
		} else if (direction == Direction.South) {
			move = new Vector3 (0,-1,0);
		} else if (direction == Direction.East) {
			move = new Vector3 (1, 0, 0);
		} else if (direction == Direction.West) {
			move = new Vector3 (-1, 0, 0);
		}
		transform.position += move;
		/*
		Implement LERP to new position
		*/
	}
}
