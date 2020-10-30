using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    [SerializeField] static int playerNum = 1;

	public void StartFirstLevel1P()
    {
        playerNum = 1;
        SceneManager.LoadScene("Level1");
    }

    public void StartFirstLevel2P()
    {
        playerNum = 2;
        SceneManager.LoadScene("Level1");
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadCreditPage()
    {
        SceneManager.LoadScene("Credit");
    }

    public int GetPlayerNum()
    {
        return playerNum;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
