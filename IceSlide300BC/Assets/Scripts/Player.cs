using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Player : MonoBehaviour {
    private GameManager gameManager;
    private Vector2Int arrayPosition;

	// Use this for initialization
	void Start () {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        arrayPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
	}
	
	// Update is called once per frame
	void Update () {
		Move();
	}

	private void Move()
	{
        //movement direction
        Vector2Int move = new Vector2Int();
        
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

        if(move == Vector2Int.zero)
            return;

        bool canMove = gameManager.PlayerMove(arrayPosition, arrayPosition + move);
        if(canMove){
            arrayPosition += move;
            transform.position += new Vector3Int(arrayPosition.x, arrayPosition.y, 0);
        }
		/*
		Implement LERP to new position
		*/
	}
}
