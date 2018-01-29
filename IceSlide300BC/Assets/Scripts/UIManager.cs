using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    // start button on main menu
    public void LoadGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("main");
    }

    // load main menu, for use with going back from other screens
    public void LoadMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("start");
    }

    // display high scores
    public void LoadHighScores()
    {
        // should we just use the main menu scene for these and show/hide different canvases? same with level select
    }

    // display level select screen
    public void LoadLevelSelect()
    {

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
