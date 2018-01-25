using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acorn : MonoBehaviour {
    private bool isMoving;

    public Vector2Int arrayPosition;
    public Vector2Int direction;
    private Vector2Int destination;

    private static float MOVE_TIME = 0.35f;
    private float moveTimer;


	// Use this for initialization
	void Start () {
        arrayPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        destination = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        direction = Vector2Int.zero;

        moveTimer = 0;
        isMoving = false;
    }

    //Set destination / future position
    public void SetDestination(Vector2Int dir, Vector2Int dest)
    {
        direction = dir;
        destination = dest;
    }

    void Move ()
    {
        if (isMoving)
        {
            LERP();
            return;
        }
        else if (arrayPosition != destination)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
            direction = Vector2Int.zero;
        }
        
    }


    // Update is called once per frame
    void Update () {
        Move();
	}

    //Same lerp from Player
    private void LERP()
    {
        //get the direction vector
        Vector2 currentLerp = destination - arrayPosition;
        //get the percent of that
        currentLerp *= (moveTimer / MOVE_TIME);
        //add the current pos to get what the position should be right now
        currentLerp += arrayPosition;
        //set the position
        transform.position = new Vector3(currentLerp.x, currentLerp.y, 0);
        //increment timer
        moveTimer += Time.deltaTime;

        //if the timer is up
        if (moveTimer > MOVE_TIME)
        {
            //reset
            moveTimer = 0;
            isMoving = false;
            //set the arrayPos
            arrayPosition = destination;
            //set the transform
            transform.position = new Vector3(arrayPosition.x, arrayPosition.y, 0);
        }
    }
}
