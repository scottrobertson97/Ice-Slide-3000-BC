using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Board : int {Floor, Wall, Acorn, Player};
public struct Level
{
	public Board[,] board;
	public List<GameObject> acorns;
    public List<GameObject> walls;
    public Vector3 playerPosition;
    public int score;
}

public class GameManager : MonoBehaviour {	
	private Level currentLevel;
    private int currentLevelID;//number of current level

    public bool isPlaying;
    public Vector2Int playerPos;

    public int moves;//number of player moves
    public int fails; //number of player fails

	//list of maps
	public List<Texture2D> maps;

	// Use this for initialization
	void Start () {
        currentLevelID = 0;
        GenerateLevel(currentLevelID);
        isPlaying = true;
	}

    //Destroys all game objects in level for Re-generation
    void ClearLevel()
    {
        foreach(GameObject acorn in currentLevel.acorns)
        {
            Destroy(acorn);
        }
        foreach (GameObject wall in currentLevel.walls)
        {
            Destroy(wall);
        }
    }

    //reset the current Level;
    void Reset()
    {
        ClearLevel();
        GenerateLevel(currentLevelID);
        Player p = FindObjectOfType<Player>();
        p.Reset();
        isPlaying = true;
    }

    //Goes to next level
    void NextLevel()
    {
        currentLevelID++;
        ClearLevel();
        GenerateLevel(currentLevelID);
        Player p = FindObjectOfType<Player>();
        p.Reset();

        //Calculate Score Here

        fails = 0;
        moves = 0;
        isPlaying = true;

    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.P))//Pause
        {
            isPlaying = !isPlaying;
        }
        if (Input.GetKeyDown(KeyCode.R) && !isPlaying)
        {
            Reset();
        }
    }


	private void GenerateLevel(int index)
    {
		currentLevel = GetComponent<LevelGenerator> ().GenerateLevel (maps[index]);
        playerPos = new Vector2Int((int)currentLevel.playerPosition.x, (int)currentLevel.playerPosition.y);
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
            moves++;
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

        if (acorn.finished) return true;

		Vector2Int destination = acornPosition;
		Vector2Int futurePos = acornPosition + direction;

		bool isOutOfBounds = false;

        if (!(futurePos.x < 0 || futurePos.x >= 10 || futurePos.y < 0 || futurePos.y >= 10)) {
            if (currentLevel.board[futurePos.x, futurePos.y] != Board.Floor)
                return false;
        }

        //Move through grid to see if acorn hits a wall or goes out of bounds
        while (true)
        {
            //if acorn is out of bounds
            if (futurePos.x < 0 || futurePos.x >= 10 || futurePos.y < 0 || futurePos.y >= 10)
            {
				//moves acorn off grid
				destination = futurePos;
				isOutOfBounds = true;
                break;
            }
            //Set acorn next to wall
			if (currentLevel.board[futurePos.x, futurePos.y] != Board.Floor)
            {
                destination = futurePos - direction;
                acorn.TakeHit();//reduce the acorn hits
                break;
            }

            futurePos += direction;
        }
            
        //Sets acorn destination

		acorn.SetDestination(direction, destination);

        //Update board for new acorn location
		if (!isOutOfBounds && !acorn.finished) currentLevel.board[destination.x, destination.y] = Board.Acorn;
        currentLevel.board[acorn.arrayPosition.x, acorn.arrayPosition.y] = Board.Floor;

		//return true, so they player moves as they push or false, if the player needs to be reset
        return CheckConditions(isOutOfBounds); ;
	}

	/// <summary>
	/// Gets the acorn at position.
	/// </summary>
	/// <returns>The acorn at position.</returns>
	/// <param name="acornPosition">Acorn position.</param>
	private Acorn GetAcornAtPosition(Vector2Int acornPosition){
		Acorn acorn = null;
		//search through the acorns in this level to find the one at the position
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

    //Check Win conditions
    private bool CheckConditions(bool isOutOfBounds)
    {
        if (isOutOfBounds)
        {
            Lose();
            return false;
        }
        else
        {
            foreach (GameObject acorn in currentLevel.acorns)
            {
                Acorn a = acorn.GetComponent<Acorn>();
                if (!a.finished)
                {
                    moves++;
                    return true;
                }
            }
            Win();
            return false;
        }
    }

    private void Lose()
    {
        isPlaying = false;
        Debug.Log("You're Loser");
        fails++;
        Reset();
        //ShowLoseSceen();
    }

    private void Win()
    {
        isPlaying = false;
        Debug.Log("You're Winner");
        NextLevel();
        //ShowWinScreen();
    }
}
