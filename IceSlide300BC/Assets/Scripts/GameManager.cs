using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Board : int {Floor, Wall, Acorn, Player};
public struct Level
{
	public Board[,] board;
	public List<GameObject> acorns;
}

public class GameManager : MonoBehaviour {
    //bool for acorn being out of bounds
    //public bool isOutBounds;

	private Level currentLevel;

	private List<Level> levels;

	// Use this for initialization
	void Start () {
        GenerateLevel();
	}
	
	// Update is called once per frame
	void Update () {

    }

    private void GenerateLevel()
    {
		currentLevel = GetComponent<LevelGenerator> ().GenerateLevel ();
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
		else if(currentLevel.board[futurePos.x, futurePos.y] == Board.Floor){
            return true;
        }
		else if(currentLevel.board[futurePos.x, futurePos.y] == Board.Acorn){
			return MoveAcorn (futurePos, futurePos - currentPos);
		}
        else
            return false;
    }

	/// <summary>
	/// Moves the acorn as far as it can in the direction, untill it hits a wall or falls off
	/// </summary>
	/// <returns><c>true</c>, if acorn was moved, <c>false</c> otherwise.</returns>
	/// <param name="direction">Direction.</param>
	private bool MoveAcorn(Vector2Int acornPosition, Vector2Int direction){
		Acorn acorn = GetAcornAtPosition (acornPosition);

		Vector2Int destination = acornPosition;
		Vector2Int futurePos = acornPosition + direction;

		bool isOutOfBounds = false;

		if (currentLevel.board [futurePos.x, futurePos.y] != Board.Floor)
			return false;

        //Move through grid to see if acorn hits a wall or goes out of bounds
        while (true)
        {
            //if acorn is out of bounds
            if (futurePos.x < 0 || futurePos.x >= 10 || futurePos.y < 0 || futurePos.y >= 10)
            {
				//moves acorn off grid
				destination = futurePos;
				isOutOfBounds = true;
                Lose();
                break;
            }
            //Set acorn next to wall
			if (currentLevel.board[futurePos.x, futurePos.y] != Board.Floor)
            {
                destination = futurePos - direction;
                break;
            }

            futurePos += direction;
        }
            
        //Sets acorn destination

		acorn.SetDestination(direction, destination);

        //Update board for new acorn location
		if (!isOutOfBounds) currentLevel.board[destination.x, destination.y] = Board.Acorn;
        currentLevel.board[acorn.arrayPosition.x, acorn.arrayPosition.y] = Board.Floor;
		//return true, so they player moves as they push
        return true;
	}

	private Acorn GetAcornAtPosition(Vector2Int acornPosition){
		Acorn acorn = null;
		foreach (GameObject a in currentLevel.acorns) {
			if ((int)a.transform.position.x == acornPosition.x && (int)a.transform.position.y == acornPosition.y) {
				acorn = a.GetComponent<Acorn> ();
			}
		}
		if (acorn == null) {
			throw new UnityException("acorn not found at that position");
		} else {
			return acorn;
		}
	}

    private void Lose()
    {

    }

    private void Win()
    {

    }
}
