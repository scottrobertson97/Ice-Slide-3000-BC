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
}

public class GameManager : MonoBehaviour {	
	private Level currentLevel;
    private int currentLevelID;//number of current level

    public bool isPlaying;
    public Vector2Int playerPos;
    public GameObject pauseMenu;

	//list of maps
	public List<Texture2D> maps;

	// Use this for initialization
	void Start () {
        currentLevelID = GameObject.Find ("LevelSelectObject").GetComponent<LevelSelectObject> ().level;
        GenerateLevel(currentLevelID);
        playerPos = new Vector2Int((int)currentLevel.playerPosition.x, (int)currentLevel.playerPosition.y);
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
        isPlaying = true;
    }

    //Goes to next level
    void NextLevel()
    {
        currentLevelID++;
        ClearLevel();
        GenerateLevel(currentLevelID);

    }

    // resume button function
    public void ResumeButton()
    {
        isPlaying = !isPlaying;
    }

    // level select button function - lose current levels progress
    public void LevelSelectButton()
    {
        // UnityEngine.SceneManagement.SceneManager.LoadScene("start");
        // would have to call level select canvas from outside this scene
        // possibly add separate level select canvas in this scene

    }

    // quit to main menu button
    public void QuitToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("start");
    }

    // handle code for displaying or hiding pause menu
    void TogglePause()
    {
        if (Input.GetKeyDown(KeyCode.P))//Pause
        {
            isPlaying = !isPlaying;
        }
        
        if (isPlaying == true)
        {
            pauseMenu.SetActive(false);
        }
        else
        {
            pauseMenu.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        // call pause method to constantly check playing status
        TogglePause();

        // check for reset too
        if (Input.GetKeyDown(KeyCode.R) && !isPlaying)
        {
            Reset();
        }
    }


	private void GenerateLevel(int index)
    {
		currentLevel = GetComponent<LevelGenerator> ().GenerateLevel (maps[index]);
		SetCameraBounds ();
    }

    /// <summary>
	/// check if the player can move and handle what happens next
    /// </summary>
    /// <returns><c>true</c>, if the player can move, <c>false</c> otherwise.</returns>
    /// <param name="currentPos">Current position of the player.</param>
    /// <param name="futurePos">Future position of the player.</param>
    public bool CanPlayerMove (Vector2Int currentPos, Vector2Int futurePos) {
        //check if the player is moving out of bounds
		if (IsOutOfBounds(futurePos.x, futurePos.y)) {
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

		if (!IsOutOfBounds(futurePos.x, futurePos.y)) {
            if (currentLevel.board[futurePos.x, futurePos.y] != Board.Floor)
                return false;
        }

        //Move through grid to see if acorn hits a wall or goes out of bounds
        while (true)
        {
            //if acorn is out of bounds
			if (IsOutOfBounds(futurePos.x, futurePos.y))
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
                acorn.TakeHit();//reduce the acorn hits
                break;
            }

            futurePos += direction;
        }
            
        //Sets acorn destination

		acorn.SetDestination(direction, destination);

        //Update board for new acorn location
		if (!isOutOfBounds) currentLevel.board[destination.x, destination.y] = Board.Acorn;
        currentLevel.board[acorn.arrayPosition.x, acorn.arrayPosition.y] = Board.Floor;

        CheckConditions(isOutOfBounds);
		//return true, so they player moves as they push
        return true;
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
    private void CheckConditions(bool isOutOfBounds)
    {
        foreach (GameObject acorn in currentLevel.acorns)
        {
            Acorn a = acorn.GetComponent<Acorn>();
            if (!a.finished)
            {
                return;
            }
        }
        Win();
    }

    private void Lose()
    {
        isPlaying = false;
        Debug.Log("You're Loser");
		//GenerateLevel (currentLevelID);
        //ShowLoseSceen();
    }

    private void Win()
    {
        isPlaying = false;
        Debug.Log("You're Winner");
        //ShowWinScreen();
    }

	private bool IsOutOfBounds(int x, int y){
		return (x < 0 || x >= currentLevel.board.GetLength(0) || y < 0 || y >= currentLevel.board.GetLength(1));
	}

	private void SetCameraBounds(){
		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ().orthographicSize = currentLevel.board.GetLength (0)/2.0f + 1;
		Vector3 camPos = GameObject.FindGameObjectWithTag ("MainCamera").transform.position;
		GameObject.FindGameObjectWithTag ("MainCamera").transform.position = new Vector3(camPos.x, currentLevel.board.GetLength (0)/2.0f - 1, camPos.z);
	}
}
