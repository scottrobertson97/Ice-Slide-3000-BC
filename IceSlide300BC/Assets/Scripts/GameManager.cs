using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    //array of things on the board
    private int[,] board = new int[10,10];

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //check if the player can move and handle what happens next
    public bool PlayerMove (Vector2Int currentPos, Vector2Int futurePos) {
        //check if the player is moving out of bounds
        if(futurePos.x < 0 || futurePos.x >= 10 || futurePos.y < 0 || futurePos.y >= 10)
            return false;
        //makes sure that the player is moving onto a tile that is open
        else if(board[futurePos.x, futurePos.y] == 0){
            return true;
        }
        else
            return false;
    }
}
