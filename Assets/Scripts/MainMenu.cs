using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public Scene game;

    public void StartGame()
    {
        SceneManager.LoadScene("MainGame");
    }

}
