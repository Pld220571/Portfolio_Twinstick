using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject _PauseScreen;

    private bool _gamePaused;

    void Start()
    {
        _gamePaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }

        if (_gamePaused)
        {
            _PauseScreen.SetActive(true);

            Time.timeScale = 0f;
        }
        else
        {
            _PauseScreen.SetActive(false);
 
            Time.timeScale = 1;
        }
    }

    public void PauseGame()
    {
        _gamePaused = true;
    }

    public void ResumeGame()
    {
        _gamePaused = false;
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
