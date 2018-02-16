using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Board : int {Floor, Wall, Acorn, Player};
public enum Gamestate : int { InProgress, Win, Lose, Paused};
public struct Level
{
	public Board[,] board;
	public List<Acorn> acorns;
    public List<GameObject> objects;
    public Vector3 playerPosition;
    public int score;
}

public class GameManager : MonoBehaviour {	
	private Level currentLevel;
    private int currentLevelID;//number of current level

	public Gamestate gamestate = Gamestate.InProgress;
    public Vector2Int playerPos;
    public GameObject pauseMenu;
    public GameObject levelSelectMenu;

    public int moves;//number of player moves
    public int fails; //number of player fails

	//list of maps
	public List<Texture2D> maps;

	// Use this for initialization
	void Start () {
        currentLevelID = GameObject.Find("LevelSelectObject").GetComponent<LevelSelectObject>().level;
        GenerateLevel(currentLevelID);
	}

    //Destroys all game objects in level for Re-generation
    void ClearLevel()
    {
        foreach(Acorn acorn in currentLevel.acorns)
        {
            DestroyImmediate(acorn);
        }
        foreach (GameObject obj in currentLevel.objects)
        {
            Destroy(obj);
        }
    }

    //reset the current Level;
    void Reset()
    {
        ClearLevel();
        GenerateLevel(currentLevelID);
        Player p = FindObjectOfType<Player>();
        p.Reset();
		gamestate = Gamestate.InProgress;
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
		gamestate = Gamestate.InProgress;

    }

    // resume button function
    public void ResumeButton()
    {
		gamestate = Gamestate.InProgress;
    }

    // level select button
    public void LevelSelectButton()
    {
        levelSelectMenu.SetActive(true);
    }

    // quit to main menu button
    public void QuitToMainMenu()
    {
        // reset selected level data
        Destroy(GameObject.Find("LevelSelectObject"));
        UnityEngine.SceneManagement.SceneManager.LoadScene("start");
    }

    // back to pause menu from level select menu
    public void BackButton()
    {
        levelSelectMenu.SetActive(false);
    }

    // handle code for displaying or hiding pause menu
    void TogglePause()
    {
		if (Input.GetKeyDown(KeyCode.P))//Pause
        {
			switch (gamestate) {
			case Gamestate.InProgress:
				pauseMenu.SetActive (true);
				gamestate = Gamestate.Paused;
				break;
			case Gamestate.Paused:
				pauseMenu.SetActive (false);
				gamestate = Gamestate.InProgress;
				break;
			}
        }
    }

    // Update is called once per frame
    void Update ()
    {
        // call pause method to constantly check playing status
        TogglePause();

        // check for reset too - when though? and we need to remember to reset move counter in UI
        if (Input.GetKeyDown(KeyCode.R) /*&& !isPlaying*/)
        {
            Reset();
        }

        bool isOut = false;
        foreach (Acorn acrn in currentLevel.acorns)
        {
            if (acrn.isMoving) return;
            if (acrn.isOut) isOut = true;
        }

        CheckConditions(isOut);
    }

    // level selection method
    public void LevelSelection()
    {
        GameObject pressedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        string level = pressedButton.name;
        Debug.Log(level);
        int levelindex = 0;
        bool tryyy = int.TryParse(level, out levelindex);

        if (tryyy == true)
        {
            Debug.Log("parsed. num = " + levelindex);
            currentLevelID = levelindex;
			levelSelectMenu.SetActive(false);

        }
        else
        {
            Debug.Log("number not parsed");
        }
        Reset();
    }

    private void GenerateLevel(int index)
    {
		currentLevel = GetComponent<LevelGenerator>().GenerateLevel(maps[index]);
		SetCameraBounds ();
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
		if (IsOutOfBounds(futurePos.x, futurePos.y)) {
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

        foreach (Acorn acn in currentLevel.acorns){
            if (acn.isMoving) { return false; }
        }

        Acorn acorn = GetAcornAtPosition (acornPosition);

        if (acorn.broken) return true;

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
                acorn.isOut = true;
                break;
            }
            //Set acorn next to wall
			if (currentLevel.board[futurePos.x, futurePos.y] != Board.Floor)
            {
                destination = futurePos - direction;
                if (currentLevel.board[futurePos.x, futurePos.y] == Board.Wall) { acorn.TakeHit(); }//reduce the acorn hits
                break;
            }

            futurePos += direction;
        }
            
        //Sets acorn destination

		acorn.SetDestination(direction, destination);
        acorn.isMoving = true;

        //Update board for new acorn location
		if (!isOutOfBounds && !acorn.broken) currentLevel.board[destination.x, destination.y] = Board.Acorn;
        currentLevel.board[acorn.arrayPosition.x, acorn.arrayPosition.y] = Board.Floor;

        //return true, so they player moves as they push or false, if the player needs to be reset
        foreach (Acorn acrn in currentLevel.acorns)
        {
            if (acorn.isMoving)
            {
                return false;
            }
        }
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
		foreach (Acorn a in currentLevel.acorns) {
			if ((int)a.transform.position.x == acornPosition.x && (int)a.transform.position.y == acornPosition.y) {
				acorn = a;
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
            foreach (Acorn acorn in currentLevel.acorns)
            {
                if (!acorn.broken)
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
		gamestate = Gamestate.Lose;
        Debug.Log("You're Loser");
        fails++;
        Reset();
        //ShowLoseSceen();
    }

    private void Win()
    {
		gamestate = Gamestate.Win;
        Debug.Log("You're Winner");
        NextLevel();
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
