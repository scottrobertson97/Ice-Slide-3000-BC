using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour {
    private GameManager gameManager;
    private Vector2Int arrayPosition;
	private Vector2Int futurePosisiton;
	private static float MOVE_TIME = 0.25f;
	private float moveTimer;
	private bool isMoving;

	// Use this for initialization
	void Start () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
		arrayPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
		moveTimer = 0;
		isMoving = false;
	}
	
	// Update is called once per frame
	void Update () {
		Move();
	}

	/// <summary>
	/// Handles player movement and input
	/// </summary>
	private void Move()
	{
		//what to so if the player is currently moving
		//this returns at the end, so if the player is moving then there is not input
		if (isMoving) {
			LERP ();
			return;
		}

        //movement direction
		Vector2Int move = new Vector2Int();
        
		//get input and the direction vector
		if (Input.GetKeyDown (KeyCode.LeftArrow)) 
		{
            move += Vector2Int.left;
		}
		if (Input.GetKeyDown (KeyCode.RightArrow)) 
		{
			move += Vector2Int.right;
		}
		if (Input.GetKeyDown (KeyCode.UpArrow)) 
		{
			move += Vector2Int.up;
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) 
		{
			move += Vector2Int.down;
		}

		//if it is zero, no input was given, so return
		if(move == Vector2Int.zero)
            return;

		//see if the player can move into that spot
        bool canMove = gameManager.CanPlayerMove(arrayPosition, arrayPosition + move);
		//if they can
        if(canMove){
			//set moving
			isMoving = true;
			//set future position
			futurePosisiton = arrayPosition + move;
        }
		/*
		Implement LERP to new position
		*/
	}

	private void LERP(){
		//get the direction vector
		Vector2 currentLerp = futurePosisiton-arrayPosition;
		//get the percent of that
		currentLerp *= (moveTimer / MOVE_TIME);
		//add the current pos to get what the position should be right now
		currentLerp += arrayPosition;
		//set the position
		transform.position = new Vector3 (currentLerp.x, currentLerp.y, 0);
		//increment timer
		moveTimer += Time.deltaTime;

		//if the timer is up
		if (moveTimer > MOVE_TIME) {
			//reset
			moveTimer = 0;
			isMoving = false;
			//set the arrayPos
			arrayPosition = futurePosisiton;
			//set the transform
			transform.position = new Vector3(arrayPosition.x, arrayPosition.y, 0);
		}
	}
}
