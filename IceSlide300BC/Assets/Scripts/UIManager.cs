using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    // objects for referencing canvases
    public GameObject menuCanvas;
    public GameObject levelSelectCanvas;
    public GameObject highScoresCanvas;
	public GameObject levelSelectObject;

	void Start () {
		DontDestroyOnLoad (levelSelectObject);
	}

    // start button on main menu
	public void LoadGame(int level)
    {
		levelSelectObject.GetComponent<LevelSelectObject>().level = level;
        UnityEngine.SceneManagement.SceneManager.LoadScene("main");
    }

    // load main menu, for use with going back from other screens
    public void LoadMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("start");
    }

    // display menu canvas after being in differnt menu
    public void backToMenu()
    {
        if (levelSelectCanvas.activeSelf == true)
        {
            levelSelectCanvas.SetActive(false);
        }
        if (highScoresCanvas.activeSelf == true)
        {
            highScoresCanvas.SetActive(false);
        }

        menuCanvas.SetActive(true);
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
        }
        else
        {
            Debug.Log("number not parsed");
        }
        LoadGame(levelindex);
    }

    // display high scores
    public void LoadHighScores()
    {
        // hide menu canvas and display high score canvas
        menuCanvas.SetActive(false);
        highScoresCanvas.SetActive(true);
    }

    // display level select screen
    public void LoadLevelSelect()
    {
        // hide menu canvas and display level select canvas
        menuCanvas.SetActive(false);
        levelSelectCanvas.SetActive(true);
    }

    // display game end screen
    public void GameOver()
    {

    }

    // exit game
    public void ExitGame()
    {
        // will work once the game is built to exe
        Application.Quit();
    }
}
