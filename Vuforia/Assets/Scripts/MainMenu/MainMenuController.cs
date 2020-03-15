using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitGame()
    {
        Debug.Log("END GAME");
        Application.Quit();
    }

    public void LoadJoinLobbyScene()
    {
        SceneManager.LoadScene("MainMenuJoinLobby");
    }
    public void LoadCreateLobbyScene()
    {
        SceneManager.LoadScene("MainMenuCreateLobby");

    }
}
