using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	//wall
	public GameObject wallPrefab;
	public GameObject acorn;

	private enum Board : int {Floor = 0, Wall = 1, Acorn = 2};
    //array of things on the board
	private Board[,] board = new Board[10,10];

	// Use this for initialization
	void Start () {
		//make 5,5 a wall
		board [5, 5] = Board.Wall;
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
		return false;
	}
}
