using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	//wall
	public GameObject wallPrefab;
	public Acorn acorn;

    //bool for acorn being out of bounds
    public bool isOutBounds;

	private enum Board : int {Floor = 0, Wall = 1, Acorn = 2};
    //array of things on the board
	private Board[,] board = new Board[10,10];

	// Use this for initialization
	void Start () {
		//make walls and set 2,2 as an acorn
		board[5, 5] = Board.Wall;
        board[7, 2] = Board.Wall;
        board[6, 9] = Board.Wall;
        board[1, 8] = Board.Wall;

        board[2, 2] = Board.Acorn;
		createWalls ();
	}
	
	// Update is called once per frame
	void Update () {

    }

	/// <summary>
	/// Creates the sprites for the walls
	/// </summary>
	void createWalls(){
		for (int x = 0; x < 10; x++) {
			for (int y = 0; y < 10; y++) {
				if (board [x, y] == Board.Wall) {
					Instantiate (wallPrefab, new Vector3 (x, y, 0), Quaternion.identity);
				}
			}
		}
	}

    /// <summary>
	/// check if the player can move and handle what happens next
    /// </summary>
    /// <returns><c>true</c>, if the player can move, <c>false</c> otherwise.</returns>
    /// <param name="currentPos">Current position of the player.</param>
    /// <param name="futurePos">Future position of the player.</param>
    public bool CanPlayerMove (Vector2Int currentPos, Vector2Int futurePos) {
        //check if the player is moving out of bounds
		if (futurePos.x < 0 || futurePos.x >= 10 || futurePos.y < 0 || futurePos.y >= 10) {
			return false;
		}
        //makes sure that the player is moving onto a tile that is open
		else if(board[futurePos.x, futurePos.y] == Board.Floor){
            return true;
        }
		else if(board[futurePos.x, futurePos.y] == Board.Acorn){
			return MoveAcorn (futurePos - currentPos);
		}
        else
            return false;
    }

	/// <summary>
	/// Moves the acorn as far as it can in the direction, untill it hits a wall or falls off
	/// </summary>
	/// <returns><c>true</c>, if acorn was moved, <c>false</c> otherwise.</returns>
	/// <param name="direction">Direction.</param>
	private bool MoveAcorn(Vector2Int direction){

        Vector2Int destination = acorn.arrayPosition;
		Vector2Int futurePos = acorn.arrayPosition + direction;

		if (board [futurePos.x, futurePos.y] != Board.Floor)
			return false;

		/*
		while (futurePos.x >= 0 || futurePos.x < 10 || futurePos.y >= 0 || futurePos.y < 10)
		is always true
		*/

        //Move through grid to see if acorn hits a wall or goes out of bounds
        while (true)
        {
            //if acorn is out of bounds
            if (futurePos.x < 0 || futurePos.x >= 10 || futurePos.y < 0 || futurePos.y >= 10)
            {
				//moves acorn off grid
				destination = futurePos; // + direction; moves it 2 off the grid
                isOutBounds = true;
                break;
            }
            //Set acorn next to wall
			if (board[futurePos.x, futurePos.y] != Board.Floor)
            {
                destination = futurePos - direction;
                break;
            }

            futurePos += direction;
        }
            
        //Sets acorn destination
        acorn.SetDestination(direction, destination);

        //Update board for new acorn location
        if (!isOutBounds) board[destination.x, destination.y] = Board.Acorn;
        board[acorn.arrayPosition.x, acorn.arrayPosition.y] = Board.Floor;
		//return true, so they player moves as they push
        return true;
	}
}
